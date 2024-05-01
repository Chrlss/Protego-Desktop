using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Management;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Protego.Navigation;
using System.Security.Principal;




namespace Protego.Pages
{
    public partial class Protection : Page
    {
        

        public static int FlashDriveScanCount { get; set; }


        private readonly HttpClient _httpClient = new HttpClient();
        private Button ScanButton;
        private ProgressBar ProgressBar;
        private TextBox StatusTextBlock;

        private readonly string quarantineFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quarantine");
        private string quarantineFolderPath = @"Quarantine";

        //private readonly string quarantineFolder = @"C:\Quarantine";

        private ManagementEventWatcher watcher;

        private List<string> processedFiles = new List<string>();

        private List<string> hashList = new List<string>();

        private bool isFlashDriveDetected = false;

        private DispatcherTimer timer;

        private int flashDriveScanCount = 0;

       

        public Protection()
        {
            InitializeComponent();

            
            //ResetScanCount();
            LoadScanCountFromSettings();
            UpdateDashboardReport();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2); // 2 second delay
            timer.Tick += Timer_Tick;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string hashDatasetFilePath = Path.Combine(baseDirectory, "Dataset", "full_sha256.txt");
            hashList = LoadHashDataset(hashDatasetFilePath);

            EnsureQuarantineFolderExists();
            LogQuarantinedFiles();

            ScanButton = FindName("ScanButtonn") as Button;
            ProgressBar = FindName("ProgressBarr") as ProgressBar;
            StatusTextBox = FindName("StatusTextBox") as TextBox;
            LogTextBox = FindName("LogTextBox") as TextBox;
            ClearLogButton = FindName("ClearLogButton") as Button;
            ClearLogButton.IsEnabled = false;

            StartMonitoringForFlashDrive();
            LogConnectedRemovableDrives();



            watcher = new ManagementEventWatcher();
            watcher.Query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3");
            watcher.EventArrived += (sender, e) =>
            {
                string driveName = e.NewEvent.GetPropertyValue("DriveName").ToString();
                string eventType = e.NewEvent.GetPropertyValue("EventType").ToString();

                Dispatcher.Invoke(() =>
                {
                    if (eventType == "2")
                    {
                        
                        Button_Click(this, new RoutedEventArgs());
                    }
                    else if (eventType == "3")
                    {
                        LogTextBox.AppendText($"Drive removed: {driveName}\n");

                    }

                });

            };
            watcher.Start();


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
        }
        private void StartFlashDriveDetectedTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5); // Set the timer interval to 5 seconds
            timer.Tick += (sender, e) =>
            {
                // Start the scan when the timer elapses
                Button_Click(this, new RoutedEventArgs ());

                // Stop the timer
                timer.Stop();
            };
            timer.Start();
        }

        private void StopFlashDriveDetectedTimer()
        {
            timer.Stop();
        }


        private void StartMonitoringForFlashDrive()
        {
            watcher = new ManagementEventWatcher();
            watcher.Query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 OR EventType = 3");
            watcher.EventArrived += (sender, e) =>
            {
                string driveName = e.NewEvent.GetPropertyValue("DriveName").ToString();
                string eventType = e.NewEvent.GetPropertyValue("EventType").ToString();

                Dispatcher.Invoke(() =>
                {
                    if (eventType == "2")
                    {
                        
                        isFlashDriveDetected = true;

                        StartFlashDriveDetectedTimer(); // Start the timer when flash drive is inserted
                    }
                    else if (eventType == "3")
                    {
                        
                        isFlashDriveDetected = false;

                        StopFlashDriveDetectedTimer(); // Stop the timer when flash drive is removed
                    }
                });
            };
            watcher.Start();
        }



        private List<string> LoadHashDataset(string filePath)
        {
            List<string> hashDataset = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                hashDataset.AddRange(lines.Select(line => line.Trim().ToLower()));
            }
            catch (Exception ex)
            {
                LogTextBox.AppendText($"Error loading hash dataset: {ex.Message}\n");
            }

            return hashDataset;
        }


        private void LogConnectedRemovableDrives()
        {
            var drives = DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Removable);
            foreach (var drive in drives)
            {
                string driveName = string.IsNullOrEmpty(drive.VolumeLabel) ? drive.Name : $"{drive.VolumeLabel} ({drive.Name})";
                string driveInfo = $"Drive detected: {driveName}";

                Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"{driveInfo}\n");
                    Button_Click(this, new RoutedEventArgs());
                });
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (watcher != null)
            {
                watcher.Stop();
                watcher.Dispose();
            }
        }
        private void EnsureQuarantineFolderExists()
        {
            if (!Directory.Exists(quarantineFolderPath))
            {
                Directory.CreateDirectory(quarantineFolderPath);


                DirectoryInfo directoryInfo = new DirectoryInfo(quarantineFolderPath);
                directoryInfo.Attributes |= FileAttributes.Hidden;


                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                directorySecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                directoryInfo.SetAccessControl(directorySecurity);
            }
        }

        private CancellationTokenSource cancellationTokenSource;
        private bool isScanning = false;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isScanning)
            {                
                return;
            }
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.SetScanInProgress(true);
            isScanning = true;

            try
            {
                string apiKey = "00fc286349bdbca1e47b6e78a07cc4791195a230d4f64a44421c6726a5126354";

                var drives = DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Removable).ToList();

                StatusTextBox.Text = "Scanning flash drive...";
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBarrPercent.Visibility = Visibility.Visible;

                int totalFilesScanned = 0;
                int totalFiles = drives.Sum(drive => Directory.GetFiles(drive.Name, "*.*", SearchOption.AllDirectories).Length);

                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;     
                isScanning = true;
                

                foreach (var drive in drives)
                {
                    string driveLetter = drive.Name;
                    var files = Directory.EnumerateFiles(driveLetter, "*.*", SearchOption.AllDirectories);

                    await Task.Run(() =>
                    {
                        foreach (var file in files)
                        {
                            if (!DriveInfo.GetDrives().Any(d => d.Name == drive.Name))
                            {
                                cancellationTokenSource.Cancel(); // Cancel the scanning process
                                return;
                            }

                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 500 * 1024 * 1024) // 500 MB in bytes
                            {
                                continue; // Skip processing this file
                            }

                            totalFilesScanned++;

                            var hashValues = GetFileHash(file);
                            bool isSuspicious = CheckFileHash(apiKey, hashValues, file);

                            Dispatcher.Invoke(() =>
                            {
                                if (!isSuspicious)
                                {
                                    LogTextBox.AppendText($"{file}: Clean\n");

                                    double progress = (double)totalFilesScanned / totalFiles * 100;
                                    AnimateProgressBar(ProgressBar.Value, progress);
                                }
                            });
                        }
                    }, cancellationToken);
                }

                isScanning = false;

                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text = $"Scan complete. Scanned {totalFilesScanned} files.";
                    ClearLogButton.IsEnabled = true;
                    ProgressBar.Visibility = Visibility.Collapsed;

                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show($"Error occurred during scan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cancellationTokenSource.Dispose();
                ProgressBarrPercent.Visibility = Visibility.Collapsed;                
                mainWindow?.SetScanInProgress(false); // Re-enable navigation after scan
                isScanning = false;
            }

            // Increment the scan count
            flashDriveScanCount++;
            SaveScanCountToSettings();

            // Update the dashboard report
            UpdateDashboardReport();
            SaveScanCountToSettings();
        }

       


        private void AnimateProgressBar(double fromValue, double toValue)
        {
            // Create a DoubleAnimation to animate the value of the progress bar
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = fromValue; // Start value
            animation.To = toValue;     // End value
            animation.Duration = TimeSpan.FromSeconds(0.5); // Animation duration (adjust as needed)

            // Set the easing function for smoother animation
            animation.EasingFunction = new CubicEase();

            // Set the target property to animate (ProgressBar.ValueProperty)
            Storyboard.SetTarget(animation, ProgressBar);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ProgressBar.ValueProperty));

            // Create a Storyboard and add the animation to it
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            // Begin the animation
            storyboard.Begin();
        }

        private void LoadScanCountFromSettings()
        {
            flashDriveScanCount = Properties.Settings.Default.FlashDriveScanCount;
        }

        private void SaveScanCountToSettings()
        {
            Properties.Settings.Default.FlashDriveScanCount = flashDriveScanCount;
            Properties.Settings.Default.Save();
        }

        private void UpdateDashboardReport()
        {
            Dispatcher.Invoke(() =>
            {
                FlashDriveScanCountLabel.Content = $"Flash Drive Scans: {flashDriveScanCount}";
                FlashDriveScanCount = flashDriveScanCount;
            });
        }


        private void ResetScanCount()
        {
            Properties.Settings.Default.FlashDriveScanCount = 0;
            Properties.Settings.Default.Save();
            UpdateDashboardReport();
        }



        private void HandleCancellation()
        {
            isScanning = false;
            Dispatcher.Invoke(() =>
            {
                StatusTextBox.Text = "Flash drive removed while scanning.";
                ProgressBar.Visibility = Visibility.Collapsed;
            });
            cancellationTokenSource.Dispose ();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            HandleCancellation();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            isScanning = false;
        }



        // Add a method to handle cancellation of the scanning process

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }
        private void ClearLog()
        {
            Dispatcher.Invoke(() =>
            {
                if (LogTextBox.Text.Length > 0 && ClearLogButton.IsEnabled)
                {
                    LogTextBox.Clear();
                    StatusTextBox.Text = "Ready to scan.";
                    ClearLogButton.IsEnabled = false;
                }
            });
        }


        private Dictionary<string, string> GetFileHash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                
                var sha256 = SHA256.Create();
                

                
                var sha256HashBytes = sha256.ComputeHash(stream);
                

                var hashValues = new Dictionary<string, string>
                {
                    
                    { "SHA-256", BitConverter.ToString(sha256HashBytes).Replace("-", "").ToLower() },
                    
                };

                return hashValues;
            }
        }


        private List<string> LoadAdditionalHashList(string filePath)
        {
            List<string> additionalHashList = new List<string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                additionalHashList.AddRange(lines.Select(line => line.Trim().ToLower()));
            }
            catch (Exception ex)
            {
                LogTextBox.AppendText($"Error loading additional hash list: {ex.Message}\n");
            }

            return additionalHashList;
        }

        private bool CheckFileHash(string apiKey, Dictionary<string, string> hashValues, string filePath)
        {
            try
            {
                string fileHash = hashValues["SHA-256"];
                if (hashList.Contains(fileHash.ToLower()))
                {

                    QuarantineFile(filePath);
                    return true;
                }

                string extension = Path.GetExtension(filePath);
                if (IsSuspiciousExtension(extension))
                {

                    QuarantineFile(filePath);
                    return true;
                }

                string url = $"https://www.virustotal.com/api/v3/files/{fileHash}";

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    request.Headers.Add("x-apikey", apiKey);

                    foreach (var hash in hashValues)
                    {
                        request.Headers.Add($"hash.{hash.Key}", hash.Value);
                    }

                    using (var response = _httpClient.SendAsync(request).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            var data = JsonSerializer.Deserialize<JsonElement>(content);

                            if (data.TryGetProperty("data", out var dataArray))
                            {
                                if (dataArray.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var item in dataArray.EnumerateArray())
                                    {
                                        var attributes = item.GetProperty("attributes");
                                        var lastAnalysisStats = attributes.GetProperty("last_analysis_stats");
                                        int positives = lastAnalysisStats.GetProperty("malicious").GetInt32();
                                        if (positives > 0)
                                        {
                                            QuarantineFile(filePath);
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    var attributes = dataArray.GetProperty("attributes");
                                    var lastAnalysisStats = attributes.GetProperty("last_analysis_stats");
                                    int positives = lastAnalysisStats.GetProperty("malicious").GetInt32();
                                    if (positives > 0)
                                    {
                                        QuarantineFile(filePath);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"Error checking file hash: {ex.Message}\n");
                });

            }

            return false;
        }

        private bool IsSuspiciousExtension(string extension)
        {
            string[] suspiciousExtensions = { ".bat", ".cpl", ".crt", ".ins", ".isp", ".ps1", ".rtf", ".crt" };
            return suspiciousExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private void QuarantineFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (processedFiles.Contains(fileName))
            {
                return;
            }

            processedFiles.Add(fileName);

            string quarantineFilePath = Path.Combine(quarantineFolder, fileName);

            try
            {
                if (!Directory.Exists(quarantineFolder))
                {
                    Directory.CreateDirectory(quarantineFolder);
                }

                if (File.Exists(quarantineFilePath))
                {
                    int count = 1;
                    string newFileName;
                    do
                    {
                        newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}({count}){Path.GetExtension(fileName)}";
                        quarantineFilePath = Path.Combine(quarantineFolder, newFileName);
                        count++;
                    } while (File.Exists(quarantineFilePath));
                }

                File.Move(filePath, quarantineFilePath);

                File.SetAttributes(quarantineFilePath, File.GetAttributes(quarantineFilePath) | FileAttributes.Hidden);

                QuarantineTextBox.Dispatcher.Invoke(() =>
                {
                    QuarantineTextBox.AppendText($"Suspicious: {Path.GetFileName(quarantineFilePath)}\n");
                });
            }
            catch (Exception ex)
            {
                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"Error quarantining file {fileName}: {ex.Message}\n");
                });
            }
        }

        private void DeleteFile(string filePath)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    File.Delete(filePath);
                    LogTextBox.AppendText($"Deleted suspicious file: {fileName}\n");
                }
                catch (Exception ex)
                {
                    LogTextBox.AppendText($"Error deleting file {filePath}: {ex.Message}\n");
                }
            });
        }


        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            string[] quarantinedFiles = QuarantineTextBox.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (quarantinedFiles.Length == 0)
            {
                MessageBox.Show("There are no files to quarantine.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }



            string restrictedFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Restricted");
            if (!Directory.Exists(restrictedFolderPath))
            {
                Directory.CreateDirectory(restrictedFolderPath);

                // Set permissions for the Restricted folder
                DirectoryInfo directoryInfo = new DirectoryInfo(restrictedFolderPath);
                directoryInfo.Attributes |= FileAttributes.Hidden;

                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                directorySecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                directoryInfo.SetAccessControl(directorySecurity);
            }

            foreach (string quarantinedFile in quarantinedFiles)
            {
                string fileName = quarantinedFile.Substring("Quarantined: ".Length).Trim();
                string filePath = Path.Combine(quarantineFolder, fileName);

                // Move the file to the Restricted folder
                string targetFilePath = Path.Combine(restrictedFolderPath, fileName);
                File.Move(filePath, targetFilePath);

                // Set permissions for the moved file
                SetFilePermissions(targetFilePath);
            }

            QuarantineTextBox.Clear();
        }

        private void SetFilePermissions(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = fileInfo.Name; // Get just the file name without the path

                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                fileInfo.SetAccessControl(fileSecurity);

                LogMessage($"{fileName}: Has been quarantined");

                // Schedule the file for deletion after 7 days
                string deletionDateFilePath = Path.Combine(quarantineFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.delete");
                if (!File.Exists(deletionDateFilePath))
                {
                    using (StreamWriter writer = File.CreateText(deletionDateFilePath))
                    {
                        writer.WriteLine(DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"));
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error setting permissions for file {filePath}: {ex.Message}");
            }
        }





        public void LogQuarantinedFiles()
        {
            if (Directory.Exists(quarantineFolder))
            {
                var files = Directory.GetFiles(quarantineFolder);
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    QuarantineTextBox.AppendText($"Quarantined: {fileName}\n");
                }
            }
        }


        private void KeepButton_Click(object sender, RoutedEventArgs e)
        {
            string[] quarantinedFiles = QuarantineTextBox.Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (quarantinedFiles.Length == 0)
            {
                MessageBox.Show("There are no quarantined files to keep.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Get the removable drive path
            string[] removableDrives = Environment.GetLogicalDrives()
                .Where(drive => new DriveInfo(drive).DriveType == DriveType.Removable)
                .ToArray();

            if (removableDrives.Length == 0)
            {
                MessageBox.Show("No removable drive found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string removableDrivePath = removableDrives[0]; // Assuming you want to use the first removable drive found
            string targetFolderPath = Path.Combine(removableDrivePath, "WARNING!_Quarantined_Files");

            if (MessageBox.Show("Are you sure you want to keep all quarantined files?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (string quarantinedFile in quarantinedFiles)
                {
                    string fileName = quarantinedFile.Substring("Quarantined: ".Length).Trim();
                    string filePath = Path.Combine(quarantineFolder, fileName);
                    MoveFileToFolder(filePath, targetFolderPath);
                }
                QuarantineTextBox.Clear();
            }
        }
        private void MoveFileToFolder(string sourceFilePath, string targetFolderPath)
        {
            try
            {
                if (!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }

                string fileName = Path.GetFileName(sourceFilePath);
                string targetFilePath = Path.Combine(targetFolderPath, fileName);

                // Ensure the file is not hidden and has normal attributes
                File.SetAttributes(sourceFilePath, FileAttributes.Normal);

                File.Copy(sourceFilePath, targetFilePath, true);

                // Get the current user's identity reference
                IdentityReference user = new NTAccount(Environment.UserDomainName, Environment.UserName).Translate(typeof(SecurityIdentifier));

                // Modify the permissions of the quarantined file to give back full control permissions
                FileInfo fileInfo = new FileInfo(targetFilePath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(user, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);

                File.Delete(sourceFilePath);

                LogMessage($"File {fileName} moved to the removable drive at {targetFolderPath}");

                // Optional: Remove the deletion date file if it exists
                string deletionDateFilePath = Path.Combine(quarantineFolder, $"{Path.GetFileNameWithoutExtension(fileName)}.delete");
                if (File.Exists(deletionDateFilePath))
                {
                    File.Delete(deletionDateFilePath);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error moving file {Path.GetFileName(sourceFilePath)} to the removable drive: {ex.Message}");
            }
        }


        private void LogMessage(string message)
        {
            // Use Dispatcher to update UI elements
            LogTextBox.Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{message}\n");
            });
        }
      
    }
}