const state = {
    locale: 'en_US',
    seed: 58933423,
    likes: 5.0,
    page: 1,
    pageSize: 10,
    viewMode: 'table',
    songs: [],
    gallerySongs: [],
    galleryPage: 1,
    isLoading: false,
    expandedIndex: null,
    expandedGalleryIndex: null,
    currentSong: null,
    audioContext: null,
    isPlaying: false,
    playbackStartTime: 0,
    scheduledSources: [],
    animationFrame: null
};

const elements = {};

document.addEventListener('DOMContentLoaded', init);

function init() {
    cacheElements();
    loadLocales();
    setupEventListeners();
    updateLikesDots();
    loadSongs();
}

function cacheElements() {
    elements.languageSelect = document.getElementById('languageSelect');
    elements.seedInput = document.getElementById('seedInput');
    elements.randomSeedBtn = document.getElementById('randomSeedBtn');
    elements.likesSlider = document.getElementById('likesSlider');
    elements.likesDots = document.getElementById('likesDots');
    elements.tableViewBtn = document.getElementById('tableViewBtn');
    elements.galleryViewBtn = document.getElementById('galleryViewBtn');
    elements.tableView = document.getElementById('tableView');
    elements.galleryView = document.getElementById('galleryView');
    elements.tableBody = document.getElementById('tableBody');
    elements.pagination = document.getElementById('pagination');
    elements.galleryGrid = document.getElementById('galleryGrid');
    elements.galleryLoader = document.getElementById('galleryLoader');
    elements.exportBtn = document.getElementById('exportBtn');
}

async function loadLocales() {
    const response = await fetch('/api/songs/locales');
    const locales = await response.json();
    elements.languageSelect.innerHTML = locales
        .map(l => `<option value="${l.code}">${l.name}</option>`)
        .join('');
    elements.languageSelect.value = state.locale;
}

function updateLikesDots() {
    const count = 20;
    const activeCount = Math.round((state.likes / 10) * count);
    let html = '';
    for (let i = 0; i < count; i++) {
        html += `<div class="likes-dot ${i < activeCount ? 'active' : ''}"></div>`;
    }
    elements.likesDots.innerHTML = html;
}

function setupEventListeners() {
    let debounceTimer;
    const debounce = (fn, delay = 300) => {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(fn, delay);
    };

    elements.languageSelect.addEventListener('change', () => {
        state.locale = elements.languageSelect.value;
        resetAndReload();
    });

    elements.seedInput.addEventListener('input', () => {
        debounce(() => {
            const val = elements.seedInput.value.trim();
            if (val && !isNaN(val)) {
                state.seed = parseInt(val);
                resetAndReload();
            }
        });
    });

    elements.randomSeedBtn.addEventListener('click', () => {
        state.seed = Math.floor(Math.random() * Number.MAX_SAFE_INTEGER);
        elements.seedInput.value = state.seed;
        resetAndReload();
    });

    elements.likesSlider.addEventListener('input', () => {
        state.likes = parseFloat(elements.likesSlider.value);
        updateLikesDots();
        debounce(() => loadSongs(true));
    });

    elements.tableViewBtn.addEventListener('click', () => switchView('table'));
    elements.galleryViewBtn.addEventListener('click', () => switchView('gallery'));

    elements.exportBtn.addEventListener('click', exportSongs);

    setupInfiniteScroll();
}

function resetAndReload() {
    state.page = 1;
    state.galleryPage = 1;
    state.gallerySongs = [];
    state.expandedIndex = null;
    stopPlayback();
    if (state.viewMode === 'gallery') {
        elements.galleryGrid.innerHTML = '';
        window.scrollTo(0, 0);
    }
    loadSongs();
}

function switchView(mode) {
    state.viewMode = mode;
    state.expandedIndex = null;
    stopPlayback();
    elements.tableViewBtn.classList.toggle('active', mode === 'table');
    elements.galleryViewBtn.classList.toggle('active', mode === 'gallery');
    elements.tableView.classList.toggle('active', mode === 'table');
    elements.galleryView.classList.toggle('active', mode === 'gallery');

    if (mode === 'gallery' && state.gallerySongs.length === 0) {
        state.galleryPage = 1;
        loadGallerySongs();
    }
}

async function loadSongs(likesOnly = false) {
    if (state.isLoading) return;
    state.isLoading = true;

    const params = new URLSearchParams({
        locale: state.locale,
        seed: state.seed,
        likes: state.likes,
        page: state.page,
        pageSize: state.pageSize
    });

    const response = await fetch(`/api/songs?${params}`);
    const data = await response.json();
    state.songs = data.songs;

    renderTable();
    renderPagination();
    state.isLoading = false;
}

async function loadGallerySongs() {
    if (state.isLoading) return;
    state.isLoading = true;
    elements.galleryLoader.style.display = 'block';

    const params = new URLSearchParams({
        locale: state.locale,
        seed: state.seed,
        likes: state.likes,
        page: state.galleryPage,
        pageSize: 12
    });

    const response = await fetch(`/api/songs?${params}`);
    const data = await response.json();

    state.gallerySongs = [...state.gallerySongs, ...data.songs];
    state.galleryPage++;

    renderGalleryAppend(data.songs);
    elements.galleryLoader.style.display = 'none';
    state.isLoading = false;
}

function renderTable() {
    let html = '';
    state.songs.forEach(song => {
        const isExpanded = state.expandedIndex === song.index;
        html += `
            <tr class="song-row ${isExpanded ? 'expanded' : ''}" data-index="${song.index}">
                <td><button class="expand-btn">
                     <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
                        <polyline points="6 9 12 15 18 9"></polyline>
                     </svg>
                </button></td>
                <td class="col-index">${song.index}</td>
                <td class="song-title">${escapeHtml(song.title)}</td>
                <td>${escapeHtml(song.artist)}</td>
                <td>${escapeHtml(song.album)}</td>
                <td>${escapeHtml(song.genre)}</td>
            </tr>
        `;
        if (isExpanded) {
            html += createExpandedRow(song);
        }
    });
    elements.tableBody.innerHTML = html;

    elements.tableBody.querySelectorAll('tr.song-row').forEach(row => {
        row.addEventListener('click', (e) => {
            // Prevent collapsing if clicking specifically on interactables inside the row (though currently none exist in the main row)
            const index = parseInt(row.dataset.index);
            toggleExpanded(index);
        });
    });

    if (state.expandedIndex !== null) {
        setupExpandedControls();
    }
}

function createExpandedRow(song) {
    const duration = formatTime(song.music.durationMs / 1000);
    const label = getRandomLabel(song.index);
    const year = getRandomYear(song.index);

    return `
        <tr class="expanded-row">
            <td colspan="6">
                <div class="expanded-row-content">
                    <div class="detail-container">
                        <!-- Left Side: Album Art & Likes -->
                        <div class="art-column">
                            <div class="album-art">
                                <canvas id="cover-${song.index}" width="160" height="160"></canvas>
                            </div>
                            <div class="like-badge">
                                ${song.likes} <span>👍</span>
                            </div>
                        </div>

                        <!-- Right Side: Info & Player -->
                        <div class="info-column">
                            <div class="song-header-large">
                                <h2 class="song-title-large">${escapeHtml(song.title)}</h2>
                                <button class="play-button-large" id="playBtn-${song.index}">▶</button>
                                <span class="volume-icon">🔊</span>
                                <span class="volume-icon dim">○</span>
                                <div class="scrubber-container">
                                    <div class="scrubber-track" id="progressBar-${song.index}">
                                        <div class="scrubber-fill" id="progressFill-${song.index}"></div>
                                    </div>
                                    <span class="time-badge" id="timeDisplay-${song.index}">0:00 / ${duration}</span>
                                </div>
                            </div>
                            
                            <div class="meta-line">
                                from <strong>${escapeHtml(song.album)}</strong> by <strong>${escapeHtml(song.artist)}</strong>
                            </div>
                            
                            <div class="label-line">
                                ${label}, ${year}
                            </div>

                            <!-- Lyrics -->
                            <div class="lyrics-section">
                                <ul class="nav nav-tabs" id="lyricsTab-${song.index}" role="tablist">
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link active" id="lyrics-tab-${song.index}" data-bs-toggle="tab" data-bs-target="#lyrics-pane-${song.index}" type="button" role="tab" aria-controls="lyrics-pane-${song.index}" aria-selected="true">Lyrics</button>
                                    </li>
                                </ul>
                                <div class="tab-content" id="lyricsTabContent-${song.index}">
                                    <div class="tab-pane fade show active" id="lyrics-pane-${song.index}" role="tabpanel" aria-labelledby="lyrics-tab-${song.index}" tabindex="0">
                                        <div class="lyrics-content" id="lyrics-${song.index}">
                                            ${song.lyrics.map((line, i) => `<p class="lyric-line" data-index="${i}">${escapeHtml(line)}</p>`).join('')}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    `;
}

function setupExpandedControls() {
    const song = state.songs.find(s => s.index === state.expandedIndex);
    if (!song) return;

    const canvas = document.getElementById(`cover-${song.index}`);
    if (canvas) {
        drawCover(canvas, song);
    }

    const playBtn = document.getElementById(`playBtn-${song.index}`);
    if (playBtn) {
        playBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            state.currentSong = song;
            togglePlayback();
        });
    }

    const progressBar = document.getElementById(`progressBar-${song.index}`);
    if (progressBar) {
        progressBar.addEventListener('click', (e) => {
            e.stopPropagation();
            if (!state.currentSong || state.currentSong.index !== song.index) return;
            const rect = progressBar.getBoundingClientRect();
            const percent = (e.clientX - rect.left) / rect.width;
            seekTo(percent * song.music.durationMs / 1000);
        });
    }
}

function toggleExpanded(index) {
    if (state.expandedIndex === index) {
        state.expandedIndex = null;
        stopPlayback();
    } else {
        state.expandedIndex = index;
        state.currentSong = state.songs.find(s => s.index === index);
        stopPlayback();
    }
    renderTable();
}

function renderPagination() {
    let html = `<button class="page-btn" ${state.page <= 1 ? 'disabled' : ''} onclick="goToPage(${state.page - 1})">«</button>`;

    const start = Math.max(1, state.page - 2);
    const end = Math.min(start + 4, 100); // Assuming decent limit, or logic similar to before

    // Simple range for now as per image (4, 5, 6)
    // Let's keep existing logic but just update classes
    const pStart = Math.max(1, state.page - 1);
    const pEnd = Math.min(999, state.page + 1);

    for (let i = pStart; i <= pEnd; i++) {
        html += `<button class="page-btn ${i === state.page ? 'active' : ''}" onclick="goToPage(${i})">${i}</button>`;
    }

    html += `<button class="page-btn" onclick="goToPage(${state.page + 1})">»</button>`;
    elements.pagination.innerHTML = html;
}

window.goToPage = function (page) {
    if (page < 1) return;
    state.page = page;
    state.expandedIndex = null;
    stopPlayback();
    loadSongs();
};

function renderGalleryAppend(songs) {
    songs.forEach(song => {
        const card = document.createElement('div');
        card.className = 'gallery-card';
        card.dataset.index = song.index;
        card.innerHTML = `
            <div class="gallery-card-cover">
                <canvas width="200" height="200"></canvas>
                <button class="gallery-play-btn" data-index="${song.index}" title="Play">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M8 5v14l11-7z"/>
                    </svg>
                </button>
            </div>
            <div class="gallery-card-info">
                <p class="gallery-card-title">${escapeHtml(song.title)}</p>
                <p class="gallery-card-artist">${escapeHtml(song.artist)}</p>
                <div class="gallery-card-meta">
                    <span>${escapeHtml(song.genre)}</span>
                    <span>${'❤'.repeat(Math.min(song.likes, 5))}</span>
                </div>
            </div>
        `;
        const canvas = card.querySelector('canvas');
        drawCover(canvas, song);
        
        // Play button click handler
        const playBtn = card.querySelector('.gallery-play-btn');
        playBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            const songToPlay = state.gallerySongs.find(s => s.index === song.index);
            if (songToPlay) {
                state.currentSong = songToPlay;
                state.expandedGalleryIndex = song.index;
                togglePlayback();
                updateGalleryPlayingState();
            }
        });
        
        // Card click handler for expansion
        card.addEventListener('click', () => {
            toggleGalleryExpanded(song.index);
        });
        
        elements.galleryGrid.appendChild(card);
    });
}

function toggleGalleryExpanded(index) {
    // Remove existing expanded detail if any
    const existingDetail = document.querySelector('.gallery-expanded-detail');
    if (existingDetail) {
        const prevIndex = parseInt(existingDetail.dataset.index);
        existingDetail.remove();
        
        // If clicking the same card, just collapse
        if (prevIndex === index) {
            state.expandedGalleryIndex = null;
            document.querySelectorAll('.gallery-card').forEach(c => c.classList.remove('expanded'));
            return;
        }
    }
    
    state.expandedGalleryIndex = index;
    const song = state.gallerySongs.find(s => s.index === index);
    if (!song) return;
    
    // Mark card as expanded
    document.querySelectorAll('.gallery-card').forEach(c => {
        c.classList.toggle('expanded', parseInt(c.dataset.index) === index);
    });
    
    // Find the card and create expanded detail
    const card = document.querySelector(`.gallery-card[data-index="${index}"]`);
    if (!card) return;
    
    const detail = createGalleryExpandedDetail(song);
    
    // Find the row end position to insert the detail
    const grid = elements.galleryGrid;
    const cards = Array.from(grid.querySelectorAll('.gallery-card'));
    const cardIndex = cards.indexOf(card);
    const cardRect = card.getBoundingClientRect();
    const gridRect = grid.getBoundingClientRect();
    
    // Find all cards in the same row
    let rowEndIndex = cardIndex;
    for (let i = cardIndex + 1; i < cards.length; i++) {
        const rect = cards[i].getBoundingClientRect();
        if (rect.top > cardRect.top + 10) break;
        rowEndIndex = i;
    }
    
    // Insert after the last card in the row
    if (rowEndIndex < cards.length - 1) {
        grid.insertBefore(detail, cards[rowEndIndex + 1]);
    } else {
        grid.appendChild(detail);
    }
    
    setupGalleryExpandedControls(song);
}

function createGalleryExpandedDetail(song) {
    const duration = formatTime(song.music.durationMs / 1000);
    const label = getRandomLabel(song.index);
    const year = getRandomYear(song.index);
    
    const detail = document.createElement('div');
    detail.className = 'gallery-expanded-detail';
    detail.dataset.index = song.index;
    
    detail.innerHTML = `
        <div class="gallery-detail-content">
            <button class="gallery-detail-close" title="Close">×</button>
            <div class="detail-container">
                <div class="art-column">
                    <div class="album-art">
                        <canvas id="gallery-cover-${song.index}" width="160" height="160"></canvas>
                    </div>
                    <div class="like-badge">
                        ${song.likes} <span>👍</span>
                    </div>
                </div>
                <div class="info-column">
                    <div class="song-header-large">
                        <h2 class="song-title-large">${escapeHtml(song.title)}</h2>
                        <button class="play-button-large" id="galleryPlayBtn-${song.index}">▶</button>
                        <span class="volume-icon">🔊</span>
                        <span class="volume-icon dim">○</span>
                        <div class="scrubber-container">
                            <div class="scrubber-track" id="galleryProgressBar-${song.index}">
                                <div class="scrubber-fill" id="galleryProgressFill-${song.index}"></div>
                            </div>
                            <span class="time-badge" id="galleryTimeDisplay-${song.index}">0:00 / ${duration}</span>
                        </div>
                    </div>
                    <div class="meta-line">
                        from <strong>${escapeHtml(song.album)}</strong> by <strong>${escapeHtml(song.artist)}</strong>
                    </div>
                    <div class="label-line">
                        ${label}, ${year}
                    </div>
                    <div class="lyrics-section">
                        <ul class="nav nav-tabs" role="tablist">
                            <li class="nav-item" role="presentation">
                                <button class="nav-link active" type="button" role="tab">Lyrics</button>
                            </li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane fade show active" role="tabpanel" tabindex="0">
                                <div class="lyrics-content" id="galleryLyrics-${song.index}">
                                    ${song.lyrics.map((line, i) => `<p class="lyric-line" data-index="${i}">${escapeHtml(line)}</p>`).join('')}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Close button handler
    detail.querySelector('.gallery-detail-close').addEventListener('click', (e) => {
        e.stopPropagation();
        toggleGalleryExpanded(song.index);
    });
    
    return detail;
}

function setupGalleryExpandedControls(song) {
    const canvas = document.getElementById(`gallery-cover-${song.index}`);
    if (canvas) {
        drawCover(canvas, song);
    }
    
    const playBtn = document.getElementById(`galleryPlayBtn-${song.index}`);
    if (playBtn) {
        playBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            state.currentSong = song;
            togglePlayback();
            updateGalleryPlayingState();
        });
    }
    
    const progressBar = document.getElementById(`galleryProgressBar-${song.index}`);
    if (progressBar) {
        progressBar.addEventListener('click', (e) => {
            e.stopPropagation();
            if (!state.currentSong || state.currentSong.index !== song.index) return;
            const rect = progressBar.getBoundingClientRect();
            const percent = (e.clientX - rect.left) / rect.width;
            seekTo(percent * song.music.durationMs / 1000);
        });
    }
}

function updateGalleryPlayingState() {
    // Update play button icons in gallery cards
    document.querySelectorAll('.gallery-play-btn').forEach(btn => {
        const index = parseInt(btn.dataset.index);
        const isPlaying = state.isPlaying && state.currentSong && state.currentSong.index === index;
        btn.innerHTML = isPlaying 
            ? '<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><path d="M6 19h4V5H6v14zm8-14v14h4V5h-4z"/></svg>'
            : '<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor"><path d="M8 5v14l11-7z"/></svg>';
    });
}

function setupInfiniteScroll() {
    window.addEventListener('scroll', () => {
        if (state.viewMode !== 'gallery') return;
        if (state.isLoading) return;

        const scrollY = window.scrollY;
        const windowHeight = window.innerHeight;
        const docHeight = document.documentElement.scrollHeight;

        if (scrollY + windowHeight >= docHeight - 200) {
            loadGallerySongs();
        }
    });
}

function drawCover(canvas, song) {
    const ctx = canvas.getContext('2d');
    const w = canvas.width;
    const h = canvas.height;
    const cover = song.cover;

    ctx.fillStyle = cover.backgroundColor;
    ctx.fillRect(0, 0, w, h);

    const rng = seededRandom(cover.patternSeed);

    ctx.globalAlpha = 0.4;
    ctx.fillStyle = cover.accentColor;

    switch (cover.patternType) {
        case 'circles':
            for (let i = 0; i < 15; i++) {
                ctx.beginPath();
                ctx.arc(rng() * w, rng() * h, rng() * (w * 0.2) + 5, 0, Math.PI * 2);
                ctx.fill();
            }
            break;
        case 'lines':
            ctx.strokeStyle = cover.accentColor;
            ctx.lineWidth = 2;
            for (let i = 0; i < 10; i++) {
                ctx.beginPath();
                ctx.moveTo(rng() * w, rng() * h);
                ctx.lineTo(rng() * w, rng() * h);
                ctx.stroke();
            }
            break;
        case 'dots':
            for (let i = 0; i < 50; i++) {
                ctx.beginPath();
                ctx.arc(rng() * w, rng() * h, rng() * 4 + 1, 0, Math.PI * 2);
                ctx.fill();
            }
            break;
        case 'waves':
            ctx.strokeStyle = cover.accentColor;
            ctx.lineWidth = 3;
            for (let j = 0; j < 3; j++) {
                ctx.beginPath();
                const offset = rng() * 10;
                for (let x = 0; x < w; x += 2) {
                    const y = h * 0.3 + j * h * 0.2 + Math.sin(x * 0.05 + offset) * 20;
                    x === 0 ? ctx.moveTo(x, y) : ctx.lineTo(x, y);
                }
                ctx.stroke();
            }
            break;
        case 'triangles':
            for (let i = 0; i < 8; i++) {
                ctx.beginPath();
                const x = rng() * w;
                const y = rng() * h;
                const size = rng() * 25 + 10;
                ctx.moveTo(x, y - size);
                ctx.lineTo(x - size, y + size);
                ctx.lineTo(x + size, y + size);
                ctx.closePath();
                ctx.fill();
            }
            break;
        case 'squares':
            for (let i = 0; i < 10; i++) {
                const size = rng() * 25 + 8;
                ctx.fillRect(rng() * w, rng() * h, size, size);
            }
            break;
        case 'gradient':
            const gradient = ctx.createLinearGradient(0, 0, w, h);
            gradient.addColorStop(0, cover.backgroundColor);
            gradient.addColorStop(1, cover.accentColor);
            ctx.globalAlpha = 0.6;
            ctx.fillStyle = gradient;
            ctx.fillRect(0, 0, w, h);
            break;
        default:
            for (let i = 0; i < 500; i++) {
                ctx.globalAlpha = rng() * 0.2;
                ctx.fillRect(rng() * w, rng() * h, 2, 2);
            }
    }

    ctx.globalAlpha = 1;
    ctx.fillStyle = '#fff';
    ctx.shadowColor = 'rgba(0,0,0,0.7)';
    ctx.shadowBlur = 3;
    ctx.textAlign = 'center';

    const titleSize = Math.max(10, Math.min(16, w * 0.12));
    ctx.font = `bold ${titleSize}px -apple-system, sans-serif`;

    const albumText = song.album.toUpperCase();
    const albumLines = wrapText(ctx, albumText, w - 16);
    let y = h * 0.5 - ((albumLines.length - 1) * titleSize * 0.6);
    albumLines.forEach(line => {
        ctx.fillText(line, w / 2, y);
        y += titleSize * 1.2;
    });

    const artistSize = Math.max(8, Math.min(11, w * 0.08));
    ctx.font = `${artistSize}px -apple-system, sans-serif`;
    ctx.fillText(song.artist.toUpperCase(), w / 2, h - 10);

    ctx.shadowBlur = 0;
}

function wrapText(ctx, text, maxWidth) {
    const words = text.split(' ');
    const lines = [];
    let current = '';

    words.forEach(word => {
        const test = current ? current + ' ' + word : word;
        if (ctx.measureText(test).width <= maxWidth) {
            current = test;
        } else {
            if (current) lines.push(current);
            current = word;
        }
    });
    if (current) lines.push(current);
    return lines;
}

function seededRandom(seed) {
    return function () {
        seed = (seed * 1103515245 + 12345) & 0x7fffffff;
        return seed / 0x7fffffff;
    };
}

function getRandomLabel(seed) {
    const labels = ['Apple Records', 'Universal Music', 'Sony Music', 'Warner Records', 'Atlantic Records', 'Capitol Records', 'Columbia Records', 'RCA Records'];
    return labels[seed % labels.length];
}

function getRandomYear(seed) {
    return 2015 + (seed % 12);
}

function togglePlayback() {
    if (state.isPlaying) {
        stopPlayback();
    } else {
        startPlayback();
    }
}

function startPlayback() {
    if (!state.currentSong) return;

    if (!state.audioContext) {
        state.audioContext = new (window.AudioContext || window.webkitAudioContext)();
    }

    const ctx = state.audioContext;
    const music = state.currentSong.music;
    const startTime = ctx.currentTime;

    state.playbackStartTime = startTime;
    state.scheduledSources = [];

    music.notes.forEach(note => {
        scheduleNote(ctx, note, startTime);
    });

    music.drums.forEach(drum => {
        scheduleDrum(ctx, drum, startTime);
    });

    music.bass.forEach(bass => {
        scheduleBass(ctx, bass, startTime);
    });

    state.isPlaying = true;
    const playBtn = document.getElementById(`playBtn-${state.currentSong.index}`);
    if (playBtn) playBtn.textContent = '⏸';
    
    const galleryPlayBtn = document.getElementById(`galleryPlayBtn-${state.currentSong.index}`);
    if (galleryPlayBtn) galleryPlayBtn.textContent = '⏸';
    
    // Update gallery card play button states
    if (typeof updateGalleryPlayingState === 'function') {
        updateGalleryPlayingState();
    }

    updateProgress();
}

function scheduleNote(ctx, note, startTime) {
    const osc = ctx.createOscillator();
    const gain = ctx.createGain();
    const filter = ctx.createBiquadFilter();

    const waveforms = ['sine', 'triangle', 'sawtooth'];
    osc.type = waveforms[Math.floor(note.note % 3)];
    osc.frequency.value = midiToFreq(note.note);

    filter.type = 'lowpass';
    filter.frequency.value = 2000;

    gain.gain.setValueAtTime(0, startTime + note.time);
    gain.gain.linearRampToValueAtTime(note.velocity * 0.3, startTime + note.time + 0.02);
    gain.gain.exponentialRampToValueAtTime(0.001, startTime + note.time + note.duration);

    osc.connect(filter);
    filter.connect(gain);
    gain.connect(ctx.destination);

    osc.start(startTime + note.time);
    osc.stop(startTime + note.time + note.duration + 0.1);

    state.scheduledSources.push(osc);
}

function scheduleDrum(ctx, drum, startTime) {
    const gain = ctx.createGain();

    if (drum.type === 'kick') {
        const osc = ctx.createOscillator();
        osc.frequency.setValueAtTime(150, startTime + drum.time);
        osc.frequency.exponentialRampToValueAtTime(40, startTime + drum.time + 0.1);
        gain.gain.setValueAtTime(drum.velocity * 0.8, startTime + drum.time);
        gain.gain.exponentialRampToValueAtTime(0.001, startTime + drum.time + 0.2);
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.start(startTime + drum.time);
        osc.stop(startTime + drum.time + 0.3);
        state.scheduledSources.push(osc);
    } else if (drum.type === 'snare') {
        const noise = ctx.createBufferSource();
        const bufferSize = ctx.sampleRate * 0.1;
        const buffer = ctx.createBuffer(1, bufferSize, ctx.sampleRate);
        const data = buffer.getChannelData(0);
        for (let i = 0; i < bufferSize; i++) {
            data[i] = Math.random() * 2 - 1;
        }
        noise.buffer = buffer;

        const filter = ctx.createBiquadFilter();
        filter.type = 'highpass';
        filter.frequency.value = 1000;

        gain.gain.setValueAtTime(drum.velocity * 0.5, startTime + drum.time);
        gain.gain.exponentialRampToValueAtTime(0.001, startTime + drum.time + 0.1);

        noise.connect(filter);
        filter.connect(gain);
        gain.connect(ctx.destination);
        noise.start(startTime + drum.time);
        noise.stop(startTime + drum.time + 0.15);
        state.scheduledSources.push(noise);
    } else if (drum.type === 'hihat') {
        const noise = ctx.createBufferSource();
        const bufferSize = ctx.sampleRate * 0.05;
        const buffer = ctx.createBuffer(1, bufferSize, ctx.sampleRate);
        const data = buffer.getChannelData(0);
        for (let i = 0; i < bufferSize; i++) {
            data[i] = Math.random() * 2 - 1;
        }
        noise.buffer = buffer;

        const filter = ctx.createBiquadFilter();
        filter.type = 'highpass';
        filter.frequency.value = 7000;

        gain.gain.setValueAtTime(drum.velocity * 0.2, startTime + drum.time);
        gain.gain.exponentialRampToValueAtTime(0.001, startTime + drum.time + 0.05);

        noise.connect(filter);
        filter.connect(gain);
        gain.connect(ctx.destination);
        noise.start(startTime + drum.time);
        noise.stop(startTime + drum.time + 0.08);
        state.scheduledSources.push(noise);
    }
}

function scheduleBass(ctx, bass, startTime) {
    const osc = ctx.createOscillator();
    const gain = ctx.createGain();
    const filter = ctx.createBiquadFilter();

    osc.type = 'sawtooth';
    osc.frequency.value = midiToFreq(bass.note);

    filter.type = 'lowpass';
    filter.frequency.value = 400;

    gain.gain.setValueAtTime(0, startTime + bass.time);
    gain.gain.linearRampToValueAtTime(0.4, startTime + bass.time + 0.01);
    gain.gain.exponentialRampToValueAtTime(0.001, startTime + bass.time + bass.duration);

    osc.connect(filter);
    filter.connect(gain);
    gain.connect(ctx.destination);

    osc.start(startTime + bass.time);
    osc.stop(startTime + bass.time + bass.duration + 0.1);

    state.scheduledSources.push(osc);
}

function midiToFreq(midi) {
    return 440 * Math.pow(2, (midi - 69) / 12);
}

function stopPlayback() {
    state.isPlaying = false;

    if (state.currentSong) {
        const playBtn = document.getElementById(`playBtn-${state.currentSong.index}`);
        if (playBtn) playBtn.textContent = '▶';
        
        const galleryPlayBtn = document.getElementById(`galleryPlayBtn-${state.currentSong.index}`);
        if (galleryPlayBtn) galleryPlayBtn.textContent = '▶';
    }

    state.scheduledSources.forEach(source => {
        try { source.stop(); } catch (e) { }
    });
    state.scheduledSources = [];

    if (state.animationFrame) {
        cancelAnimationFrame(state.animationFrame);
        state.animationFrame = null;
    }
    
    // Update gallery card play button states
    if (typeof updateGalleryPlayingState === 'function') {
        updateGalleryPlayingState();
    }
}

function seekTo(time) {
    stopPlayback();
    state.playbackStartTime = state.audioContext.currentTime - time;
    startPlayback();
}

function updateProgress() {
    if (!state.isPlaying || !state.currentSong) return;

    const ctx = state.audioContext;
    const elapsed = ctx.currentTime - state.playbackStartTime;
    const duration = state.currentSong.music.durationMs / 1000;
    const songIndex = state.currentSong.index;

    // Table view elements
    const progressFill = document.getElementById(`progressFill-${songIndex}`);
    const timeDisplay = document.getElementById(`timeDisplay-${songIndex}`);
    const lyricsContainer = document.getElementById(`lyrics-${songIndex}`);
    
    // Gallery view elements
    const galleryProgressFill = document.getElementById(`galleryProgressFill-${songIndex}`);
    const galleryTimeDisplay = document.getElementById(`galleryTimeDisplay-${songIndex}`);
    const galleryLyricsContainer = document.getElementById(`galleryLyrics-${songIndex}`);

    const timeText = `${formatTime(elapsed)} / ${formatTime(duration)}`;

    if (elapsed >= duration) {
        stopPlayback();
        if (progressFill) progressFill.style.width = '100%';
        if (timeDisplay) timeDisplay.textContent = `${formatTime(duration)} / ${formatTime(duration)}`;
        if (galleryProgressFill) galleryProgressFill.style.width = '100%';
        if (galleryTimeDisplay) galleryTimeDisplay.textContent = `${formatTime(duration)} / ${formatTime(duration)}`;
        updateGalleryPlayingState();
        return;
    }

    const percent = (elapsed / duration) * 100;
    
    // Update table view
    if (progressFill) progressFill.style.width = `${percent}%`;
    if (timeDisplay) timeDisplay.textContent = timeText;
    
    // Update gallery view
    if (galleryProgressFill) galleryProgressFill.style.width = `${percent}%`;
    if (galleryTimeDisplay) galleryTimeDisplay.textContent = timeText;

    if (lyricsContainer) {
        updateLyricHighlight(lyricsContainer, elapsed, duration);
    }
    if (galleryLyricsContainer) {
        updateLyricHighlight(galleryLyricsContainer, elapsed, duration);
    }

    state.animationFrame = requestAnimationFrame(updateProgress);
}

function updateLyricHighlight(container, elapsed, duration) {
    const lines = container.querySelectorAll('.lyric-line');
    const totalLines = lines.length;
    if (totalLines === 0) return;

    const lineInterval = duration / totalLines;
    const currentLineIndex = Math.floor(elapsed / lineInterval);

    lines.forEach((line, i) => {
        line.classList.remove('active', 'past');
        if (i < currentLineIndex) {
            line.classList.add('past');
        } else if (i === currentLineIndex) {
            line.classList.add('active');
        }
    });
}

function formatTime(seconds) {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

async function exportSongs() {
    const btn = elements.exportBtn;
    const originalContent = btn.innerHTML;
    
    // Show loading state
    btn.disabled = true;
    btn.innerHTML = `
        <svg class="spinner" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10" stroke-dasharray="32" stroke-dashoffset="12"/>
        </svg>
        <span>Exporting...</span>
    `;

    try {
        const params = new URLSearchParams({
            locale: state.locale,
            seed: state.seed,
            likes: state.likes,
            page: state.viewMode === 'table' ? state.page : 1,
            pageSize: state.viewMode === 'table' ? state.pageSize : Math.min(state.gallerySongs.length, 50) || 12
        });

        const response = await fetch(`/api/export?${params}`);
        
        if (!response.ok) {
            throw new Error('Export failed');
        }

        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `seedsound_export_${state.seed}_page${state.page}.zip`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
    } catch (error) {
        console.error('Export error:', error);
        alert('Failed to export songs. Please try again.');
    } finally {
        // Restore button state
        btn.disabled = false;
        btn.innerHTML = originalContent;
    }
}
