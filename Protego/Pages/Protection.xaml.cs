using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Management;
using System.Diagnostics;
using System.Windows.Threading;

namespace Protego.Pages
{
    public partial class Protection : Page
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Button ScanButton;
        private ProgressBar ProgressBar;
        private TextBox StatusTextBlock; // Reference the RichTextBox control

        private readonly string quarantineFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Quarantine");

        private string quarantineFolderPath = @"C:\Quarantine"; // Specify the path to the quarantine folder

        private ManagementEventWatcher watcher;

        private List<string> processedFiles = new List<string>(); // List to store processed file names

        
        public Protection()
        {
            InitializeComponent();

            EnsureQuarantineFolderExists();
            LogQuarantinedFiles();

            ScanButton = FindName("ScanButtonn") as Button;
            ProgressBar = FindName("ProgressBarr") as ProgressBar;
            StatusTextBox = FindName("StatusTextBox") as TextBox;
            LogTextBox = FindName("LogTextBox") as TextBox;
            ClearLogButton = FindName("ClearLogButton") as Button;

            ClearLogButton.IsEnabled = false; // Initially disable clear button

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
                        Button_Click(this, new RoutedEventArgs()); // Run malware scan for the inserted drive
                    }
                    else if (eventType == "3")
                    {
                        LogTextBox.AppendText($"Drive removed: {driveName}\n");
                        // Add your code here to handle drive removal
                    }
                });
            };
            watcher.Start();

            

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
                });

                // Run malware scan for each connected removable drive
                Dispatcher.Invoke(() =>
                {
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

                // Make the folder hidden
                DirectoryInfo directoryInfo = new DirectoryInfo(quarantineFolderPath);
                directoryInfo.Attributes |= FileAttributes.Hidden;

                // Remove read and execute permissions
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                directorySecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                directoryInfo.SetAccessControl(directorySecurity);
            }
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Replace with your actual API key
                string apiKey = "00fc286349bdbca1e47b6e78a07cc4791195a230d4f64a44421c6726a5126354";

                // Scan for removable drives
                var drives = DriveInfo.GetDrives().Where(drive => drive.DriveType == DriveType.Removable);

                StatusTextBox.Text = "Scanning flash drive...";
                ProgressBar.Visibility = Visibility.Visible;

                int totalFilesScanned = 0;
                int totalFiles = drives.Sum(drive => Directory.GetFiles(drive.Name, "*.*", SearchOption.AllDirectories).Length);

                foreach (var drive in drives)
                {
                    string driveLetter = drive.Name;
                    var files = Directory.EnumerateFiles(driveLetter, "*.*", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        totalFilesScanned++;

                        string fileHash = GetFileHash(file);
                        bool isSuspicious = await CheckFileHash(apiKey, fileHash, file); // Pass the file path here

                        Dispatcher.Invoke(() =>
                        {
                            LogTextBox.AppendText($"{file}: {(isSuspicious ? "Suspicious" : "Clean")}\n");

                            // Update progress bar
                            double progress = (double)totalFilesScanned / totalFiles * 100;
                            ProgressBar.Value = progress;
                        });
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    StatusTextBox.Text = $"Scan complete. Scanned {totalFilesScanned} files.";

                    // Enable clear button only if there are log entries
                    ClearLogButton.IsEnabled = LogTextBox.Text.Length > 0;
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    StatusTextBox.Text = $"Error: {ex.Message}";
                    MessageBox.Show($"Error occurred during scan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }


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
                    ClearLogButton.IsEnabled = false; // Disable clear button after clearing
                }
            });
        }


        private string GetFileHash(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private async Task<bool> CheckFileHash(string apiKey, string fileHash, string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (extension.Equals(".bat", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".cpl", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".crt", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".ins", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".isp", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".ps1", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".rtf", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".crt", StringComparison.OrdinalIgnoreCase))
            {
                // Mark specified file extensions as suspicious without checking with VirusTotal
                QuarantineFile(filePath);
                return true;
            }

            string url = $"https://www.virustotal.com/api/v3/files/{fileHash}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("x-apikey", apiKey);

                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

                        if (data.ContainsKey("positives"))
                        {
                            int positives = Convert.ToInt32(data["positives"]);
                            if (positives > 0)
                            {
                                QuarantineFile(filePath);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        private void QuarantineFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath); // Get only the file name without the path

            if (processedFiles.Contains(fileName)) // Check if the file has already been processed
            {
                return; // Skip processing if the file is already in the list
            }

            processedFiles.Add(fileName); // Add the file to the list of processed files

            string quarantineFilePath = Path.Combine(quarantineFolder, fileName);

            try
            {
                if (!Directory.Exists(quarantineFolder))
                {
                    Directory.CreateDirectory(quarantineFolder);
                }

                if (File.Exists(quarantineFilePath))
                {
                    // File with the same name already exists in the quarantine folder
                    string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}";
                    quarantineFilePath = Path.Combine(quarantineFolder, newFileName);
                }

                // Log suspicious file before moving it to quarantine
                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"{fileName}: Suspicious\n");
                });

                // Move the file to the quarantine folder
                File.Move(filePath, quarantineFilePath);

                // Set the file attributes to hidden
                File.SetAttributes(quarantineFilePath, File.GetAttributes(quarantineFilePath) | FileAttributes.Hidden);

                // Remove read and execute permissions
                var fileInfo = new FileInfo(quarantineFilePath);
                var fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.ReadAndExecute, AccessControlType.Deny));
                fileInfo.SetAccessControl(fileSecurity);

                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"Quarantined suspicious file: {Path.GetFileName(quarantineFilePath)}\n");
                });

                // Append quarantined file name to QuarantineTextBox
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
                    string fileName = Path.GetFileName(filePath); // Get only the file name without the path
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
                    string fileName = quarantinedFile.Substring("Quarantined: ".Length).Trim(); // Trim to remove leading and trailing whitespaces
                    string filePath = Path.Combine(quarantineFolder, fileName);
                    DeleteFile(filePath);
                }
                QuarantineTextBox.Clear(); // Clear all entries from QuarantineTextBox
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

            if (MessageBox.Show("Are you sure you want to keep all quarantined files for 7 days?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (string quarantinedFile in quarantinedFiles)
                {
                    string filePath = quarantinedFile.Substring("Quarantined: ".Length).Trim(); // Trim to remove leading and trailing whitespaces
                    KeepFileFor7Days(filePath);
                }
                QuarantineTextBox.Clear(); // Clear all entries from QuarantineTextBox
            }
        }

        private async void KeepFileFor7Days(string filePath)
        {
            try
            {
                string quarantineFilePath = Path.Combine(quarantineFolder, Path.GetFileName(filePath));

                // Move the file to the quarantine folder if it's not already there
                if (!File.Exists(quarantineFilePath))
                {
                    File.Move(filePath, quarantineFilePath);
                }

                // Calculate the date 7 days from now
                DateTime deletionDate = DateTime.Now.AddDays(7);

                // Write the deletion date to a file in the quarantine folder
                string deletionDateFilePath = Path.Combine(quarantineFolder, $"{Path.GetFileNameWithoutExtension(filePath)}.delete");
                await File.WriteAllTextAsync(deletionDateFilePath, deletionDate.ToString());

                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"File {Path.GetFileName(filePath)} will be deleted on {deletionDate}\n");
                });
            }
            catch (Exception ex)
            {
                LogTextBox.Dispatcher.Invoke(() =>
                {
                    LogTextBox.AppendText($"Error keeping file {Path.GetFileName(filePath)}: {ex.Message}\n");
                });
            }
        }

       
    }
    
}

