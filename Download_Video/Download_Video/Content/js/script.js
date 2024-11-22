document.getElementById('preview-button').addEventListener('click', function () {
  const videoUrl = document.getElementById('video-url').value.trim();
  const previewSection = document.getElementById('preview-section');
  const previewContent = document.getElementById('preview-content');
  const qualitySelect = document.getElementById('video-quality');
  const downloadOptions = document.getElementById('download-options');

  // Reset nội dung trước đó
  previewContent.innerHTML = '';
  qualitySelect.innerHTML = '';
  downloadOptions.innerHTML = '';
  previewSection.style.display = 'none';

  // Hàm kiểm tra URL
  const isYouTube = (url) => /(?:youtube\.com\/watch\?v=|youtu\.be\/)/.test(url);
  const isTikTok = (url) => /tiktok\.com\/(?:@[^/]+\/video\/|v\/)/.test(url);
  const isInstagram = (url) => /instagram\.com\/(?:p|reel|tv)\//.test(url);
  const isTwitter = (url) => /twitter\.com\/.*\/status\/\d+/.test(url);

  if (isYouTube(videoUrl)) {
    // Xử lý URL YouTube
    const videoId = videoUrl.match(/(?:v=|youtu\.be\/)([A-Za-z0-9_-]+)/)[1];
    previewContent.innerHTML = `
            <iframe width="100%" height="315" src="https://www.youtube.com/embed/${videoId}" frameborder="0" allowfullscreen></iframe>
        `;
    ['144p', '360p', '720p', '1080p'].forEach((quality) => {
      const option = document.createElement('option');
      option.value = quality;
      option.textContent = quality;
      qualitySelect.appendChild(option);
    });
    downloadOptions.innerHTML = `<button onclick="downloadYouTubeVideo('${videoId}')">Tải xuống video YouTube</button>`;
  } else if (isTikTok(videoUrl)) {
    // Xử lý URL TikTok
    previewContent.innerHTML =
      `<p>Video TikTok không thể nhúng. Vui lòng sử dụng nút bên dưới để tải xuống.</p>`;
    ['360p', '720p'].forEach((quality) => {
      const option = document.createElement('option');
      option.value = quality;
      option.textContent = quality;
      qualitySelect.appendChild(option);
    });
    downloadOptions.innerHTML = `<button onclick="downloadTikTokVideo('${videoUrl}')">Tải xuống video TikTok</button>`;
  } else if (isInstagram(videoUrl)) {
    // Xử lý URL Instagram
    previewContent.innerHTML = `<p>Video Instagram không thể nhúng. Vui lòng sử dụng nút bên dưới để tải xuống.</p>`;
    downloadOptions.innerHTML = `<button onclick="downloadInstagramVideo('${videoUrl}')">Tải xuống video Instagram</button>`;
  } else if (isTwitter(videoUrl)) {
    // Xử lý URL Twitter
    previewContent.innerHTML = `<p>Video Twitter không thể nhúng. Vui lòng sử dụng nút bên dưới để tải xuống.</p>`;
    downloadOptions.innerHTML = `<button onclick="downloadTwitterVideo('${videoUrl}')">Tải xuống video Twitter</button>`;
  } else {
    // URL không hợp lệ hoặc không được hỗ trợ
    previewContent.innerHTML = `<p>Liên kết không hợp lệ hoặc nền tảng không được hỗ trợ. Vui lòng nhập liên kết từ YouTube, TikTok, Instagram hoặc Twitter.</p>`;
  }

  // Hiển thị khu vực preview
  previewSection.style.display = 'block';
});

// Hàm giả lập tải video
function downloadYouTubeVideo(videoId) {
  alert(`Tải xuống video YouTube ID: ${videoId}`);
}

function downloadTikTokVideo(url) {
  alert(`Tải xuống video TikTok: ${url}`);
}

function downloadInstagramVideo(url) {
  alert(`Tải xuống video Instagram: ${url}`);
}

function downloadTwitterVideo(url) {
  alert(`Tải xuống video Twitter: ${url}`);
}
