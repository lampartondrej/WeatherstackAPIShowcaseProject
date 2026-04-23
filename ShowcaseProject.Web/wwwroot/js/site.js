// Function to set background class based on local time
function setTimeBasedBackground() {
    const body = document.getElementById('dynamic-body');
    if (!body) return;

    const currentHour = new Date().getHours();
    body.classList.remove('morning', 'midday', 'evening', 'night');

    if (currentHour >= 5 && currentHour < 10) {
        body.classList.add('morning');
    } else if (currentHour >= 10 && currentHour < 17) {
        body.classList.add('midday');
    } else if (currentHour >= 17 && currentHour < 21) {
        body.classList.add('evening');
    } else {
        body.classList.add('night');
    }
}

// Function to map weather codes/descriptions to custom icons
function getWeatherIcon(weatherCode, weatherDescription, isDay) {
    // Weatherstack weather codes mapping
    const isDayTime = isDay === 'yes';

    // Map based on weather code (primary) or description (fallback)
    const code = parseInt(weatherCode);

    // Sunny/Clear
    if ([113].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('clear'))) {
        return isDayTime ? '☀️' : '🌙';
    }
    // Partly Cloudy
    if ([116].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('partly cloudy'))) {
        return isDayTime ? '⛅' : '🌙';
    }
    // Cloudy
    if ([119, 122].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('cloudy'))) {
        return '☁️';
    }
    // Overcast
    if ([143, 248, 260].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('overcast'))) {
        return '☁️';
    }
    // Mist/Fog
    if ([143, 248, 260].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('mist') || weatherDescription && weatherDescription.toLowerCase().includes('fog'))) {
        return '🌫️';
    }
    // Light Rain/Drizzle
    if ([176, 263, 266, 281, 293, 296].includes(code) || (weatherDescription && (weatherDescription.toLowerCase().includes('drizzle') || weatherDescription.toLowerCase().includes('light rain')))) {
        return '🌦️';
    }
    // Rain
    if ([299, 302, 305, 308, 353, 356, 359].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('rain') && !weatherDescription.toLowerCase().includes('light'))) {
        return '🌧️';
    }
    // Heavy Rain
    if ([308, 359].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('heavy rain'))) {
        return '⛈️';
    }
    // Snow/Light Snow
    if ([179, 227, 230, 323, 326, 329, 332, 335, 338, 368, 371].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('snow'))) {
        return '❄️';
    }
    // Sleet/Ice
    if ([182, 185, 281, 284, 311, 314, 317, 350, 362, 365, 374, 377].includes(code) || (weatherDescription && (weatherDescription.toLowerCase().includes('sleet') || weatherDescription.toLowerCase().includes('ice')))) {
        return '🌨️';
    }
    // Thunder
    if ([200, 386, 389, 392, 395].includes(code) || (weatherDescription && weatherDescription.toLowerCase().includes('thunder'))) {
        return '⛈️';
    }
    // Blizzard/Heavy Snow
    if ([338, 395].includes(code) || (weatherDescription && (weatherDescription.toLowerCase().includes('blizzard') || weatherDescription.toLowerCase().includes('heavy snow')))) {
        return '🌨️';
    }

    // Default fallback
    return isDayTime ? '🌤️' : '🌙';
}

document.addEventListener('DOMContentLoaded', () => {
    setTimeBasedBackground();
    checkSavedLocation();
});
setInterval(setTimeBasedBackground, 60000);

// Tab navigation functionality
document.querySelectorAll('.nav-tab').forEach(tab => {
    tab.addEventListener('click', function (e) {
        e.preventDefault();

        document.querySelectorAll('.nav-tab').forEach(t => t.classList.remove('active'));
        this.classList.add('active');

        const tabName = this.dataset.tab;
        const currentView = document.getElementById('current-weather-view');
        const forecastView = document.getElementById('forecast-weather-view');
        const savedLocation = localStorage.getItem('weatherLocation');

        if (tabName === 'current') {
            if (currentView) currentView.classList.add('active');
            if (forecastView) forecastView.classList.remove('active');
            if (savedLocation) {
                fetchCurrentWeather(savedLocation);
            }
        } else if (tabName === 'forecast') {
            if (currentView) currentView.classList.remove('active');
            if (forecastView) forecastView.classList.add('active');
            if (savedLocation) {
                fetchForecastWeather(savedLocation);
            }
        }
    });
});

// Location popup elements
const locationPopup = document.getElementById('location-popup-overlay');
const locationInput = document.getElementById('location-input');
const saveLocationBtn = document.getElementById('save-location-btn');
const changeLocationBtn = document.getElementById('change-location-btn');
const currentLocationDisplay = document.getElementById('current-location-display');
const popupTitle = document.getElementById('popup-title');
const popupSubtitle = document.getElementById('popup-subtitle');

let isChangingLocation = false;

// Weather API functions
async function fetchCurrentWeather(location) {
    try {
        const response = await fetch(`/api/WeatherApi/current/${encodeURIComponent(location)}`);
        if (response.ok) {
            const data = await response.json();
            updateCurrentWeatherUI(data);
            return data;
        } else {
            throw new Error(`Server error: ${response.status}`);
        }
    } catch (error) {
        console.error('Error fetching current weather:', error);
        throw error; // re-throw so loadWeatherDataWithRetry can catch it
    }
}

async function fetchForecastWeather(location) {
    try {
        const response = await fetch(`/api/WeatherApi/forecast/${encodeURIComponent(location)}`);
        if (response.ok) {
            const data = await response.json();
            updateForecastWeatherUI(data);
            return data;
        } else {
            throw new Error(`Server error: ${response.status}`);
        }
    } catch (error) {
        console.error('Error fetching forecast weather:', error);
        throw error; // re-throw so loadWeatherDataWithRetry can catch it
    }
}

function updateCurrentWeatherUI(data) {
    if (!data || !data.current) return;

    const current = data.current;
    const location = data.location;
    const astro = current.astro;

    setElementText('current-temperature', `${current.temperature}°C`);
    setElementText('current-description', current.weather_descriptions?.[0] || '--');
    setElementText('current-location-name', location ? `${location.name}, ${location.country}` : '--');

    // Use custom weather icon mapping
    const iconEl = document.getElementById('current-weather-icon');
    if (iconEl) {
        const customIcon = getWeatherIcon(
            current.weather_code, 
            current.weather_descriptions?.[0], 
            current.is_day
        );
        iconEl.textContent = customIcon;
    }

    setElementText('current-wind-speed', `${current.wind_speed || '--'} km/h`);
    setElementText('current-humidity', `${current.humidity || '--'}%`);
    setElementText('current-feelslike', `${current.feelslike || '--'}°C`);
    setElementText('current-pressure', `${current.pressure || '--'} mb`);
    setElementText('current-visibility', `${current.visibility || '--'} km`);
    setElementText('current-cloudcover', `${current.cloudcover || '--'}%`);

    if (astro) {
        setElementText('current-sunrise', astro.sunrise || '--:--');
        setElementText('current-sunset', astro.sunset || '--:--');
        setElementText('current-moonrise', astro.moonrise || '--:--');
        setElementText('current-moonset', astro.moonset || '--:--');
        setElementText('current-moon-phase', astro.moon_phase || '--');
        setElementText('current-moon-illumination', `${astro.moon_illumination || '--'}%`);
    }
}

function updateForecastWeatherUI(data) {
    if (!data || !data.current) return;

    const current = data.current;
    const location = data.location;
    const astro = current.astro;
    const forecast = data.forecast;

    setElementText('forecast-temperature', `${current.temperature}°C`);
    setElementText('forecast-description', current.weather_descriptions?.[0] || '--');
    setElementText('forecast-location-name', location ? `${location.name}, ${location.country}` : '--');

    // Use custom weather icon mapping
    const iconEl = document.getElementById('forecast-weather-icon');
    if (iconEl) {
        const customIcon = getWeatherIcon(
            current.weather_code, 
            current.weather_descriptions?.[0], 
            current.is_day
        );
        iconEl.textContent = customIcon;
    }

    setElementText('forecast-wind-speed', `${current.wind_speed || '--'} km/h`);
    setElementText('forecast-humidity', `${current.humidity || '--'}%`);
    setElementText('forecast-feelslike', `${current.feelslike || '--'}°C`);
    setElementText('forecast-pressure', `${current.pressure || '--'} mb`);
    setElementText('forecast-visibility', `${current.visibility || '--'} km`);
    setElementText('forecast-cloudcover', `${current.cloudcover || '--'}%`);

    if (astro) {
        setElementText('forecast-sunrise', astro.sunrise || '--:--');
        setElementText('forecast-sunset', astro.sunset || '--:--');
        setElementText('forecast-moonrise', astro.moonrise || '--:--');
        setElementText('forecast-moonset', astro.moonset || '--:--');
        setElementText('forecast-moon-phase', astro.moon_phase || '--');
        setElementText('forecast-moon-illumination', `${astro.moon_illumination || '--'}%`);
    }

    if (forecast) {
        const days = Object.keys(forecast);
        days.slice(0, 5).forEach((date, index) => {
            const dayData = forecast[date];
            const dayNum = index + 1;
            const dayName = new Date(date).toLocaleDateString('en-US', { weekday: 'short' });

            setElementText(`forecast-day-${dayNum}-name`, dayName);
            setElementText(`forecast-day-${dayNum}-temp`, `${dayData.avgtemp || '--'}°`);

            // Use custom weather icon for forecast days
            const dayIcon = document.getElementById(`forecast-day-${dayNum}-icon`);
            if (dayIcon && dayData) {
                // Use the first hourly data or default values for the day
                const customIcon = getWeatherIcon(
                    dayData.weather_code || current.weather_code, 
                    dayData.weather_descriptions?.[0] || current.weather_descriptions?.[0], 
                    'yes' // Assume daytime for forecast days
                );
                dayIcon.textContent = customIcon;
            }
        });
    }
}

function setElementText(id, text) {
    const el = document.getElementById(id);
    if (el) el.textContent = text;
}

async function loadWeatherData(location) {
    console.log('Loading weather data for:', location);
    await loadWeatherDataWithRetry(location);
}

function checkSavedLocation() {
    const savedLocation = localStorage.getItem('weatherLocation');
    if (savedLocation) {
        hideLocationPopup();
        updateLocationDisplay(savedLocation);
        loadWeatherData(savedLocation);
    } else {
        isChangingLocation = false;
        updatePopupText();
        showLocationPopup();
    }
}

function updateLocationDisplay(location) {
    if (currentLocationDisplay) {
        currentLocationDisplay.textContent = location;
    }
}

function updatePopupText() {
    if (isChangingLocation) {
        if (popupTitle) popupTitle.textContent = 'Change Location';
        if (popupSubtitle) popupSubtitle.textContent = 'Enter a new location for weather data';
    } else {
        if (popupTitle) popupTitle.textContent = 'Welcome!';
        if (popupSubtitle) popupSubtitle.textContent = 'Enter your location to get weather data';
    }
}

function showLocationPopup() {
    if (locationPopup) {
        locationPopup.classList.remove('hidden');
        const savedLocation = localStorage.getItem('weatherLocation');
        if (isChangingLocation && savedLocation && locationInput) {
            locationInput.value = savedLocation;
            locationInput.select();
        } else if (locationInput) {
            locationInput.value = '';
        }
        setTimeout(() => {
            if (locationInput) locationInput.focus();
        }, 100);
    }
}

function hideLocationPopup() {
    if (locationPopup) {
        locationPopup.classList.add('hidden');
    }
    isChangingLocation = false;
}

function saveLocation() {
    const location = locationInput.value.trim();
    if (location) {
        localStorage.setItem('weatherLocation', location);
        updateLocationDisplay(location);
        hideLocationPopup();
        loadWeatherData(location);
    } else {
        locationInput.style.animation = 'shake 0.5s';
        setTimeout(() => {
            locationInput.style.animation = '';
        }, 500);
    }
}

// Event listeners
if (saveLocationBtn) {
    saveLocationBtn.addEventListener('click', saveLocation);
}

if (locationInput) {
    locationInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            saveLocation();
        }
    });
}

if (changeLocationBtn) {
    changeLocationBtn.addEventListener('click', function() {
        isChangingLocation = true;
        updatePopupText();
        showLocationPopup();
    });
}

if (locationPopup) {
    locationPopup.addEventListener('click', function(e) {
        if (e.target === locationPopup && isChangingLocation) {
            hideLocationPopup();
        }
    });
}

document.addEventListener('keydown', function(e) {
    if (e.key === 'Escape' && isChangingLocation) {
        hideLocationPopup();
    }
});

// ── Fetch Overlay helpers ────────────────────────────────────
const fetchOverlay  = document.getElementById('fetch-overlay');
const fetchLoading  = document.getElementById('fetch-loading');
const fetchError    = document.getElementById('fetch-error');
const fetchErrorMsg = document.getElementById('fetch-error-message');
const fetchLabel    = document.querySelector('.fetch-label');
const fetchRetryBtn = document.getElementById('fetch-retry-btn');

const MAX_RETRIES = 3;

function showFetchLoading(attempt = 1, max = MAX_RETRIES) {
    fetchLoading.classList.remove('hidden');
    fetchError.classList.add('hidden');
    fetchOverlay.classList.remove('hidden');
    if (fetchLabel) {
        fetchLabel.textContent = attempt > 1
            ? `Retrying… (attempt ${attempt} of ${max})`
            : 'Fetching weather data…';
    }
}

function showFetchError(message) {
    fetchLoading.classList.add('hidden');
    fetchErrorMsg.textContent = message || 'The weather service could not be reached. Please try again later.';
    fetchError.classList.remove('hidden');
}

function hideFetchOverlay() {
    fetchOverlay.classList.add('hidden');
}

async function loadWeatherDataWithRetry(location, attempt = 1) {
    showFetchLoading(attempt, MAX_RETRIES);

    try {
        const results = await Promise.all([
            fetchCurrentWeather(location),
            // fetchForecastWeather(location)
        ]);

        const allFailed = results.every(r => r === null);
        if (allFailed) {
            throw new Error('No data returned from the weather service.');
        }

        hideFetchOverlay();
    } catch (err) {
        if (attempt < MAX_RETRIES) {
            // Brief pause before next automatic retry
            await new Promise(resolve => setTimeout(resolve, 1200));
            await loadWeatherDataWithRetry(location, attempt + 1);
        } else {
            showFetchError(err.message);
        }
    }
}

// Retry button — restarts the full 3-attempt sequence from scratch
if (fetchRetryBtn) {
    fetchRetryBtn.addEventListener('click', () => {
        const savedLocation = localStorage.getItem('weatherLocation');
        if (savedLocation) {
            loadWeatherDataWithRetry(savedLocation);
        }
    });
}
