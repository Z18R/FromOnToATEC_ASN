using System;
using System.IO;
using Renci.SshNet;

namespace SFTPconnect
{
    internal static class Program
    {
        static void Main()
        {
            string localDirectory = @"C:\xml\intransit\"; // Destination directory where files will be downloaded
            string sftpDirectory = "/fromON/ASN/Test/";

            try
            {
                using (var client = new SftpClient("sftp.onsemi.com", "ATEC_sftp", "wstHSKXBSU7"))
                {
                    client.Connect();

                    var files = client.ListDirectory(sftpDirectory);

                    foreach (var file in files)
                    {
                        // Skip directories and symbolic links
                        if (file.IsDirectory || file.IsSymbolicLink) continue;

                        string fileName = file.Name;
                        string remoteFilePath = sftpDirectory + fileName; // Ensure there's a "/" between directory and file
                        string localFilePath = Path.Combine(localDirectory, fileName);

                        // Download the file
                        try
                        {
                            using (var stream = File.Create(localFilePath))
                            {
                                client.DownloadFile(remoteFilePath, stream);
                            }
                           // Console.WriteLine($"Downloaded file: {fileName}");
                        }
                        catch (Exception downloadEx)
                        {
                            //Console.WriteLine($"Failed to download file {fileName}: {downloadEx.Message}");
                            continue; // Skip deletion if download failed
                        }

                        // Delete the file
                        try
                        {
                            client.DeleteFile(remoteFilePath);
                            //Console.WriteLine($"Deleted file: {fileName}");
                        }
                        catch (Exception deleteEx)
                        {
                            //Console.WriteLine($"Failed to delete file {fileName}: {deleteEx.Message}");
                        }
                    }

                    client.Disconnect();
                }

                //Console.WriteLine("All files processed successfully!");
            }
            catch (Exception ex)
            {
               // Console.WriteLine("An error occurred: " + ex.Message);
            }

            Environment.Exit(0);
        }
    }
}
