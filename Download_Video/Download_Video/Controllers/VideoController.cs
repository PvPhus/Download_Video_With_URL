using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
                // Sử dụng yt-dlp để tải video
                string outputPath = GenerateDownload(videoUrl);

                if (string.IsNullOrEmpty(outputPath))
                {
                    ViewBag.Message = "Không thể xử lý video từ URL này!";
                    return View("Index");
                }

                // Trả về thông tin đường dẫn tải xuống
                ViewBag.LinkPreview = Url.Content(outputPath); // URL để xem trước
                ViewBag.LinkDownload = Url.Content(outputPath); // URL để tải xuống
                ViewBag.Message = "Tải video thành công!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("Index");
        }

        // Hàm xóa video sau khoảng thời gian (xx giây)
        private void ScheduleFileDeletion(string filePath, int delayInSeconds)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(delayInSeconds));
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); // Xóa file
                        Debug.WriteLine($"File đã được xóa: {filePath}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Không thể xóa file: {ex.Message}");
                }
            });
        }

        // Hàm xử lý tải video bằng yt-dlp
        private string GenerateDownload(string videoUrl)
        {
            string ytdlpPath = @"D:\Get_Url_Download_Short_Video\Download_Video\Download_Video\yt-dlp.exe";

            // Kiểm tra yt-dlp.exe có tồn tại không
            if (!System.IO.File.Exists(ytdlpPath))
            {
                throw new FileNotFoundException("Không tìm thấy yt-dlp.exe tại đường dẫn: " + ytdlpPath);
            }

            string outputDirectory = Server.MapPath("~/Downloads/");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Đảm bảo đường dẫn không chứa ký tự không hợp lệ
            string outputFile = Path.Combine(outputDirectory, "%(title)s.%(ext)s");

            // Cấu hình tham số yt-dlp
            string arguments = $"\"{videoUrl}\" -o \"{outputFile}\" " +
                               "--merge-output-format mp4 " +
                               "--format bestvideo[ext=mp4]+bestaudio[ext=m4a]/mp4 " +
                               "--no-playlist " +
                               "--restrict-filenames " + // Xử lý tên tệp an toàn
                               "--retries infinite " +
                               "--no-part " +
                               "--quiet --no-warnings";

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
                    // Tìm file tải xuống
                    var downloadedFiles = Directory.GetFiles(outputDirectory, "*.mp4");
                    if (downloadedFiles.Length > 0)
                    {
                        string downloadedFile = downloadedFiles[0];

                        // Lên lịch xóa file sau 30 giây
                        ScheduleFileDeletion(downloadedFile, 30);

                        // Trả về đường dẫn URL tương đối
                        return "~/Downloads/" + Path.GetFileName(downloadedFile);
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
