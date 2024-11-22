using System;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace VideoDownloader.Controllers
{
    public class VideoController : Controller
    {
        // Hiển thị giao diện nhập URL
        public ActionResult Index()
        {
            return View();
        }

        // Xử lý URL
        [HttpPost]
        public ActionResult ProcessVideo(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                ViewBag.Message = "URL không hợp lệ!";
                return View("Index");
            }

            try
            {
                // Sử dụng yt-dlp để lấy liên kết tải trực tiếp hoặc tải xuống
                string outputPath = GenerateDownload(videoUrl);

                if (string.IsNullOrEmpty(outputPath))
                {
                    ViewBag.Message = "Không thể xử lý video từ URL này!";
                    return View("Index");
                }

                // Trả về thông tin cho người dùng
                ViewBag.PreviewUrl = outputPath;
                ViewBag.DownloadLink = outputPath; // Đường dẫn tải xuống
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("Index");
        }

        // Hàm xử lý tải video bằng yt-dlp
        private string GenerateDownload(string videoUrl)
        {
            string ytdlpPath = @"D:\Get_Url_Download_Short_Video\Download_Video\Download_Video\yt-dlp.exe";

            string outputDirectory = Server.MapPath("~/Downloads/");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFile = $"{outputDirectory}\\%(title)s.%(ext)s";

            string arguments = $"\"{videoUrl}\" -o \"{outputFile}\" " +
                               "--merge-output-format mp4 " +
                               "--format bestvideo[ext=mp4]+bestaudio[ext=m4a]/mp4 " +
                               "--no-playlist " +
                               "--retries infinite " +
                               "--no-part " +
                               "--quiet --no-warnings " +
                               "--progress ";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ytdlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    var downloadedFile = Directory.GetFiles(outputDirectory, "*.mp4");
                    if (downloadedFile.Length > 0)
                    {
                        return "/Downloads/" + Path.GetFileName(downloadedFile[0]);
                    }
                    else
                    {
                        throw new Exception("Không tìm thấy file được tải xuống.");
                    }
                }
                else
                {
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception("yt-dlp lỗi: " + error);
                }
            }
        }
    }
}


