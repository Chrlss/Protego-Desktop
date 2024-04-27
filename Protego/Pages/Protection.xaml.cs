using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Management;
using System.Windows.Threading;




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

        private string quarantineFolderPath = @"C:\Quarantine";

        private ManagementEventWatcher watcher;

        private List<string> processedFiles = new List<string>();

        private List<string> hashList = new List<string>();

        private bool isFlashDriveDetected = false;

        private DispatcherTimer timer;

        private int flashDriveScanCount = 0;

        public Protection()
        {
            InitializeComponent();

            ResetScanCount();
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
                        LogTextBox.AppendText($"Drive inserted: {driveName}\n");
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
                        LogTextBox.AppendText($"Drive inserted: {driveName}\n");
                        isFlashDriveDetected = true;

                        StartFlashDriveDetectedTimer(); // Start the timer when flash drive is inserted
                    }
                    else if (eventType == "3")
                    {
                        LogTextBox.AppendText($"Drive removed: {driveName}\n");
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
                string driveInfo = $"Drive detected: {drive.Name}\n";

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

            try
            {
                string apiKey = "00fc286349bdbca1e47b6e78a07cc4791195a230d4f64a44421c6726a5126354";

                var drives = DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Removable).ToList();

                StatusTextBox.Text = "Scanning flash drive...";
                ProgressBar.Visibility = Visibility.Visible;

                int totalFilesScanned = 0;
                int totalFiles = drives.Sum(drive => Directory.GetFiles(drive.Name, "*.*", SearchOption.AllDirectories).Length);

                cancellationTokenSource = new CancellationTokenSource();
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                isScanning = true;

                foreach (var drive in drives)
                {
                    // Check if the flash drive is still connected
                    if (!DriveInfo.GetDrives().Any(d => d.Name == drive.Name))
                    {
                        StatusTextBox.Text = "Flash drive removed during scanning.";
                        ProgressBar.Visibility = Visibility.Collapsed;
                        return;
                    }

                    string driveLetter = drive.Name;
                    var files = Directory.EnumerateFiles(driveLetter, "*.*", SearchOption.AllDirectories);

                    await Task.Run(() =>
                    {
                        foreach (var file in files)
                        {
                            // Check if the flash drive is still connected
                            if (!DriveInfo.GetDrives().Any(d => d.Name == drive.Name))
                            {
                                cancellationTokenSource.Cancel(); // Cancel the scanning process
                                return;
                            }

                            // Continue processing the file
                            FileInfo fileInfo = new FileInfo(file);
                            if (fileInfo.Length > 500 * 1024 * 1024) // 500 MB in bytes
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    LogTextBox.AppendText($" ");
                                });
                                continue; // Skip processing this file
                            }

                            totalFilesScanned++;

                            var hashValues = GetFileHash(file);
                            bool isSuspicious = CheckFileHash(apiKey, hashValues, file);

                            Dispatcher.Invoke(() =>
                            {
                                LogTextBox.AppendText($"{file}: {(isSuspicious ? "Suspicious" : "Clean")}\n");

                                double progress = (double)totalFilesScanned / totalFiles * 100;
                                ProgressBar.Value = progress;
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
            }

            // Increment the scan count
            flashDriveScanCount++;
            SaveScanCountToSettings();

            // Update the dashboard report
            UpdateDashboardReport();
            SaveScanCountToSettings();
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
                StatusTextBox.Text = "Flash drive removed during scanning.";
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
                var md5 = MD5.Create();
                var sha1 = SHA1.Create();
                var sha256 = SHA256.Create();
                var sha512 = SHA512.Create();

                var md5HashBytes = md5.ComputeHash(stream);
                var sha1HashBytes = sha1.ComputeHash(stream);
                var sha256HashBytes = sha256.ComputeHash(stream);
                var sha512HashBytes = sha512.ComputeHash(stream);

                var hashValues = new Dictionary<string, string>
                {
                    { "MD5", BitConverter.ToString(md5HashBytes).Replace("-", "").ToLower() },
                    { "SHA-1", BitConverter.ToString(sha1HashBytes).Replace("-", "").ToLower() },
                    { "SHA-256", BitConverter.ToString(sha256HashBytes).Replace("-", "").ToLower() },
                    { "SHA-512", BitConverter.ToString(sha512HashBytes).Replace("-", "").ToLower() }
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

                    string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
                    quarantineFilePath = Path.Combine(quarantineFolder, newFileName);
                }


                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"{fileName}: Suspicious\n");
                });


                File.Move(filePath, quarantineFilePath);


                File.SetAttributes(quarantineFilePath, File.GetAttributes(quarantineFilePath) | FileAttributes.Hidden);


                var fileInfo = new FileInfo(quarantineFilePath);
                var fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                fileInfo.SetAccessControl(fileSecurity);

                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"Quarantined suspicious file: {Path.GetFileName(quarantineFilePath)}\n");
                });


                QuarantineTextBox.Dispatcher.Invoke(() =>
                {
                    QuarantineTextBox.AppendText($"Quarantined: {Path.GetFileName(quarantineFilePath)}\n");
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
                MessageBox.Show("There are no files to clean.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Are you sure you want to clean all quarantined files?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (string quarantinedFile in quarantinedFiles)
                {
                    string fileName = quarantinedFile.Substring("Quarantined: ".Length).Trim();
                    string filePath = Path.Combine(quarantineFolder, fileName);
                    DeleteFile(filePath);
                }
                QuarantineTextBox.Clear();
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

            if (MessageBox.Show("Are you sure you want to keep all quarantined files? Quarantined files will be deleted in 7 days.", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (string quarantinedFile in quarantinedFiles)
                {
                    string filePath = quarantinedFile.Substring("Quarantined: ".Length).Trim();
                    KeepFileFor7Days(filePath);
                }
                QuarantineTextBox.Clear();
            }
        }

        private async Task KeepFileFor7Days(string filePath)
        {
            try
            {
                string quarantineFilePath = Path.Combine(quarantineFolder, Path.GetFileName(filePath));
                DateTime deletionDate = DateTime.Now.AddDays(7);

                await Task.Run(() =>
                {
                    File.Move(filePath, quarantineFilePath);
                    string deletionDateFilePath = Path.Combine(quarantineFolder, $"{Path.GetFileNameWithoutExtension(filePath)}.delete");
                    File.WriteAllText(deletionDateFilePath, deletionDate.ToString());
                });

                LogMessage($"File {Path.GetFileName(filePath)} will be deleted on {deletionDate}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error keeping file {Path.GetFileName(filePath)}: {ex.Message}");
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



