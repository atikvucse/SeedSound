namespace SeedSound.Core.Localization;

public class LocaleData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BogusLocale { get; set; } = string.Empty;
    public List<string> TitlePrefixes { get; set; } = new();
    public List<string> TitleNouns { get; set; } = new();
    public List<string> TitleSuffixes { get; set; } = new();
    public List<string> TitleAdjectives { get; set; } = new();
    public List<string> BandPrefixes { get; set; } = new();
    public List<string> BandNouns { get; set; } = new();
    public List<string> BandSuffixes { get; set; } = new();
    public List<string> AlbumWords { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public List<string> ReviewPhrases { get; set; } = new();
    public List<string> LyricPhrases { get; set; } = new();
    public string SingleText { get; set; } = "Single";
}

public static class LocaleDataStore
{
    private static readonly Dictionary<string, LocaleData> Locales = new()
    {
        ["en_US"] = new LocaleData
        {
            Code = "en_US",
            Name = "English (US)",
            BogusLocale = "en",
            TitlePrefixes = new List<string>
            {
                "The", "My", "Your", "Our", "Electric", "Neon", "Golden", "Silver", "Midnight",
                "Crystal", "Velvet", "Broken", "Lost", "Eternal", "Sacred", "Secret", "Silent",
                "Wild", "Burning", "Frozen", "Dancing", "Fading", "Rising", "Falling", "Endless"
            },
            TitleNouns = new List<string>
            {
                "Heart", "Dream", "Sky", "Fire", "Rain", "Storm", "Night", "Day", "Star", "Moon",
                "Sun", "Road", "River", "Ocean", "Mountain", "Forest", "City", "Light", "Shadow",
                "Thunder", "Lightning", "Wind", "Flame", "Ice", "Diamond", "Rose", "Wolf", "Eagle",
                "Dragon", "Phoenix", "Angel", "Ghost", "Spirit", "Soul", "Mind", "Time", "Love",
                "Hope", "Fear", "Pain", "Joy", "Tear", "Smile", "Kiss", "Touch", "Voice", "Song"
            },
            TitleSuffixes = new List<string>
            {
                "Tonight", "Forever", "Again", "Away", "Inside", "Outside", "Above", "Below",
                "Beyond", "Within", "Without", "Alone", "Together", "Now", "Then", "Here", "There"
            },
            TitleAdjectives = new List<string>
            {
                "Beautiful", "Dangerous", "Mysterious", "Powerful", "Gentle", "Fierce", "Calm",
                "Wild", "Free", "Bound", "Lost", "Found", "Broken", "Whole", "Empty", "Full"
            },
            BandPrefixes = new List<string>
            {
                "The", "Electric", "Neon", "Cosmic", "Digital", "Analog", "Atomic", "Nuclear",
                "Sonic", "Super", "Ultra", "Mega", "Hyper", "Cyber", "Techno", "Retro", "Neo"
            },
            BandNouns = new List<string>
            {
                "Wolves", "Tigers", "Lions", "Eagles", "Hawks", "Ravens", "Crows", "Snakes",
                "Dragons", "Phoenix", "Unicorns", "Knights", "Kings", "Queens", "Princes",
                "Pirates", "Ninjas", "Samurai", "Warriors", "Soldiers", "Rebels", "Outlaws",
                "Ghosts", "Spirits", "Angels", "Demons", "Vampires", "Zombies", "Aliens",
                "Robots", "Machines", "Engines", "Rockets", "Stars", "Planets", "Galaxies",
                "Circuits", "Waves", "Frequencies", "Echoes", "Shadows", "Flames", "Storms"
            },
            BandSuffixes = new List<string>
            {
                "Band", "Crew", "Squad", "Gang", "Collective", "Society", "Club", "Union",
                "Alliance", "Order", "Guild", "Assembly", "Project", "Experience", "Sound"
            },
            AlbumWords = new List<string>
            {
                "Journey", "Voyage", "Adventure", "Quest", "Mission", "Odyssey", "Expedition",
                "Chronicles", "Tales", "Stories", "Legends", "Myths", "Dreams", "Visions",
                "Reflections", "Echoes", "Shadows", "Lights", "Colors", "Seasons", "Elements",
                "Dimensions", "Realms", "Worlds", "Universes", "Horizons", "Frontiers", "Edges",
                "Boundaries", "Limits", "Infinity", "Eternity", "Moments", "Memories", "Future"
            },
            Genres = new List<string>
            {
                "Rock", "Pop", "Electronic", "Hip-Hop", "R&B", "Jazz", "Blues", "Country",
                "Folk", "Classical", "Metal", "Punk", "Indie", "Alternative", "Soul", "Funk",
                "Disco", "House", "Techno", "Trance", "Ambient", "Reggae", "Latin", "World"
            },
            ReviewPhrases = new List<string>
            {
                "This track takes you on an incredible journey through sound and emotion.",
                "A masterpiece that blends genres seamlessly with innovative production.",
                "The artist showcases their incredible range and musical prowess here.",
                "An unforgettable melody that will stay with you long after listening.",
                "Bold and experimental while remaining deeply accessible and catchy.",
                "Pure musical genius captured in every note and beat.",
                "This song redefines what modern music can achieve.",
                "A sonic experience that transcends traditional boundaries.",
                "Every element perfectly crafted to create auditory bliss.",
                "The production quality here is absolutely stunning."
            },
            LyricPhrases = new List<string>
            {
                "Every beat reminds me of you, tearing me apart",
                "In the million suns that shine, you're the brightest star",
                "At the break of dawn, you're all I want, no matter how far",
                "Oh Melanie, I try to move on",
                "Dancing in the moonlight, feeling so alive",
                "We were young and free, nothing could stop us now",
                "Take my hand and never let go",
                "Through the storm we'll find our way home",
                "Your love is like a fire burning bright",
                "I'll wait forever if it takes that long",
                "The night is young and so are we",
                "Lost in your eyes, I found my paradise",
                "Running through the rain, chasing dreams again",
                "You're the melody in my heart",
                "Together we can reach the stars above"
            },
            SingleText = "Single"
        },
        ["de_DE"] = new LocaleData
        {
            Code = "de_DE",
            Name = "German (Germany)",
            BogusLocale = "de",
            TitlePrefixes = new List<string>
            {
                "Der", "Die", "Das", "Mein", "Dein", "Unser", "Elektrisch", "Neon", "Golden",
                "Silber", "Mitternacht", "Kristall", "Samt", "Gebrochen", "Verloren", "Ewig",
                "Heilig", "Geheim", "Still", "Wild", "Brennend", "Gefroren", "Tanzend"
            },
            TitleNouns = new List<string>
            {
                "Herz", "Traum", "Himmel", "Feuer", "Regen", "Sturm", "Nacht", "Tag", "Stern",
                "Mond", "Sonne", "Weg", "Fluss", "Ozean", "Berg", "Wald", "Stadt", "Licht",
                "Schatten", "Donner", "Blitz", "Wind", "Flamme", "Eis", "Diamant", "Rose",
                "Wolf", "Adler", "Drache", "Phönix", "Engel", "Geist", "Seele", "Zeit", "Liebe",
                "Hoffnung", "Angst", "Schmerz", "Freude", "Träne", "Lächeln", "Kuss", "Stimme"
            },
            TitleSuffixes = new List<string>
            {
                "Heute Nacht", "Für Immer", "Wieder", "Weg", "Innen", "Außen", "Oben", "Unten",
                "Jenseits", "Innerhalb", "Ohne", "Allein", "Zusammen", "Jetzt", "Dann", "Hier"
            },
            TitleAdjectives = new List<string>
            {
                "Schön", "Gefährlich", "Mysteriös", "Mächtig", "Sanft", "Wild", "Ruhig",
                "Frei", "Gebunden", "Verloren", "Gefunden", "Gebrochen", "Ganz", "Leer", "Voll"
            },
            BandPrefixes = new List<string>
            {
                "Die", "Elektrische", "Neon", "Kosmische", "Digitale", "Analoge", "Atomare",
                "Sonische", "Super", "Ultra", "Mega", "Hyper", "Cyber", "Techno", "Retro", "Neo"
            },
            BandNouns = new List<string>
            {
                "Wölfe", "Tiger", "Löwen", "Adler", "Falken", "Raben", "Krähen", "Schlangen",
                "Drachen", "Phönixe", "Einhörner", "Ritter", "Könige", "Königinnen", "Prinzen",
                "Piraten", "Ninjas", "Samurai", "Krieger", "Soldaten", "Rebellen", "Gesetzlose",
                "Geister", "Engel", "Dämonen", "Vampire", "Zombies", "Außerirdische", "Roboter",
                "Maschinen", "Motoren", "Raketen", "Sterne", "Planeten", "Galaxien", "Wellen"
            },
            BandSuffixes = new List<string>
            {
                "Band", "Crew", "Truppe", "Bande", "Kollektiv", "Gesellschaft", "Klub", "Union",
                "Allianz", "Orden", "Gilde", "Versammlung", "Projekt", "Erlebnis", "Klang"
            },
            AlbumWords = new List<string>
            {
                "Reise", "Fahrt", "Abenteuer", "Suche", "Mission", "Odyssee", "Expedition",
                "Chroniken", "Geschichten", "Erzählungen", "Legenden", "Mythen", "Träume",
                "Visionen", "Reflexionen", "Echos", "Schatten", "Lichter", "Farben", "Jahreszeiten",
                "Elemente", "Dimensionen", "Reiche", "Welten", "Universen", "Horizonte", "Grenzen"
            },
            Genres = new List<string>
            {
                "Rock", "Pop", "Elektronisch", "Hip-Hop", "R&B", "Jazz", "Blues", "Country",
                "Folk", "Klassik", "Metal", "Punk", "Indie", "Alternative", "Soul", "Funk",
                "Disco", "House", "Techno", "Trance", "Ambient", "Reggae", "Schlager", "Volksmusik"
            },
            ReviewPhrases = new List<string>
            {
                "Dieser Track nimmt dich mit auf eine unglaubliche Reise durch Klang und Emotion.",
                "Ein Meisterwerk, das Genres nahtlos mit innovativer Produktion verbindet.",
                "Der Künstler zeigt hier seine unglaubliche Bandbreite und musikalisches Können.",
                "Eine unvergessliche Melodie, die lange nach dem Hören bei dir bleibt.",
                "Mutig und experimentell, dabei tief zugänglich und eingängig.",
                "Pures musikalisches Genie in jeder Note und jedem Beat eingefangen.",
                "Dieser Song definiert neu, was moderne Musik erreichen kann.",
                "Ein Klangerlebnis, das traditionelle Grenzen überschreitet.",
                "Jedes Element perfekt gefertigt für akustische Glückseligkeit.",
                "Die Produktionsqualität hier ist absolut atemberaubend."
            },
            LyricPhrases = new List<string>
            {
                "Jeder Schlag erinnert mich an dich, zerreißt mich",
                "In den Millionen Sonnen, die scheinen, bist du der hellste Stern",
                "Im Morgengrauen bist du alles was ich will, egal wie weit",
                "Oh Melanie, ich versuche weiterzumachen",
                "Tanzen im Mondlicht, fühle mich so lebendig",
                "Wir waren jung und frei, nichts konnte uns aufhalten",
                "Nimm meine Hand und lass niemals los",
                "Durch den Sturm finden wir unseren Weg nach Hause",
                "Deine Liebe ist wie ein hell brennendes Feuer",
                "Ich werde ewig warten wenn es so lange dauert",
                "Die Nacht ist jung und wir auch",
                "Verloren in deinen Augen fand ich mein Paradies",
                "Rennend durch den Regen, Träume jagend wieder",
                "Du bist die Melodie in meinem Herzen",
                "Zusammen können wir die Sterne erreichen"
            },
            SingleText = "Single"
        },
        ["uk_UA"] = new LocaleData
        {
            Code = "uk_UA",
            Name = "Ukrainian (Ukraine)",
            BogusLocale = "uk",
            TitlePrefixes = new List<string>
            {
                "Моє", "Твоє", "Наше", "Електричний", "Неоновий", "Золотий", "Срібний",
                "Опівнічний", "Кришталевий", "Оксамитовий", "Зламаний", "Загублений", "Вічний",
                "Священний", "Таємний", "Тихий", "Дикий", "Палаючий", "Замерзлий", "Танцюючий"
            },
            TitleNouns = new List<string>
            {
                "Серце", "Мрія", "Небо", "Вогонь", "Дощ", "Буря", "Ніч", "День", "Зірка",
                "Місяць", "Сонце", "Дорога", "Річка", "Океан", "Гора", "Ліс", "Місто", "Світло",
                "Тінь", "Грім", "Блискавка", "Вітер", "Полумя", "Лід", "Діамант", "Троянда",
                "Вовк", "Орел", "Дракон", "Фенікс", "Ангел", "Привид", "Душа", "Час", "Любов",
                "Надія", "Страх", "Біль", "Радість", "Сльоза", "Усмішка", "Поцілунок", "Голос"
            },
            TitleSuffixes = new List<string>
            {
                "Сьогодні Вночі", "Назавжди", "Знову", "Геть", "Всередині", "Зовні", "Вгорі",
                "Внизу", "За Межами", "Всередині", "Без", "Наодинці", "Разом", "Зараз", "Тоді"
            },
            TitleAdjectives = new List<string>
            {
                "Красивий", "Небезпечний", "Таємничий", "Потужний", "Ніжний", "Лютий", "Спокійний",
                "Дикий", "Вільний", "Звязаний", "Загублений", "Знайдений", "Зламаний", "Цілий"
            },
            BandPrefixes = new List<string>
            {
                "Електричні", "Неонові", "Космічні", "Цифрові", "Аналогові", "Атомні", "Ядерні",
                "Звукові", "Супер", "Ультра", "Мега", "Гіпер", "Кібер", "Техно", "Ретро", "Нео"
            },
            BandNouns = new List<string>
            {
                "Вовки", "Тигри", "Леви", "Орли", "Яструби", "Круки", "Ворони", "Змії",
                "Дракони", "Феніkси", "Єдинороги", "Лицарі", "Королі", "Королеви", "Принци",
                "Пірати", "Ніндзя", "Самураї", "Воїни", "Солдати", "Повстанці", "Розбійники",
                "Привиди", "Духи", "Ангели", "Демони", "Вампіри", "Зомбі", "Прибульці", "Роботи",
                "Машини", "Двигуни", "Ракети", "Зірки", "Планети", "Галактики", "Хвилі"
            },
            BandSuffixes = new List<string>
            {
                "Бенд", "Команда", "Загін", "Банда", "Колектив", "Товариство", "Клуб", "Союз",
                "Альянс", "Орден", "Гільдія", "Збори", "Проект", "Досвід", "Звук"
            },
            AlbumWords = new List<string>
            {
                "Подорож", "Мандрівка", "Пригода", "Пошук", "Місія", "Одіссея", "Експедиція",
                "Хроніки", "Казки", "Історії", "Легенди", "Міфи", "Мрії", "Візії", "Відображення",
                "Ехо", "Тіні", "Вогні", "Кольори", "Пори Року", "Елементи", "Виміри", "Світи"
            },
            Genres = new List<string>
            {
                "Рок", "Поп", "Електроніка", "Хіп-Хоп", "R&B", "Джаз", "Блюз", "Кантрі",
                "Фолк", "Класика", "Метал", "Панк", "Інді", "Альтернатива", "Соул", "Фанк",
                "Диско", "Хаус", "Техно", "Транс", "Ембієнт", "Реггі", "Українська Естрада"
            },
            ReviewPhrases = new List<string>
            {
                "Цей трек веде вас у неймовірну подорож звуком та емоціями.",
                "Шедевр, що бездоганно поєднує жанри з інноваційним продакшеном.",
                "Артист демонструє тут свій неймовірний діапазон та музичну майстерність.",
                "Незабутня мелодія, яка залишиться з вами надовго після прослуховування.",
                "Сміливий та експериментальний, залишаючись глибоко доступним і цікавим.",
                "Чистий музичний геній у кожній ноті та биті.",
                "Ця пісня переосмислює те, чого може досягти сучасна музика.",
                "Звуковий досвід, що виходить за традиційні межі.",
                "Кожен елемент ідеально створений для звукового блаженства.",
                "Якість продакшена тут абсолютно приголомшлива."
            },
            LyricPhrases = new List<string>
            {
                "Кожен удар нагадує мені про тебе, розриває мене",
                "Серед мільйонів сонць, що сяють, ти найяскравіша зірка",
                "На світанку ти все що я хочу, незалежно від відстані",
                "О Мелані, я намагаюся рухатися далі",
                "Танцюючи у місячному світлі, почуваюсь таким живим",
                "Ми були молоді і вільні, ніщо не могло нас зупинити",
                "Візьми мою руку і ніколи не відпускай",
                "Крізь бурю ми знайдемо шлях додому",
                "Твоя любов як яскраво палаючий вогонь",
                "Я чекатиму вічно якщо це займе стільки часу",
                "Ніч молода і ми теж",
                "Загублений у твоїх очах я знайшов свій рай",
                "Біжучи крізь дощ, знову ганяючись за мріями",
                "Ти мелодія в моєму серці",
                "Разом ми можемо досягти зірок вгорі"
            },
            SingleText = "Сингл"
        }
    };

    public static LocaleData GetLocale(string code)
    {
        return Locales.TryGetValue(code, out var locale) ? locale : Locales["en_US"];
    }

    public static IEnumerable<(string Code, string Name)> GetAvailableLocales()
    {
        return Locales.Select(l => (l.Key, l.Value.Name));
    }
}
