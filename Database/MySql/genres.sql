USE `ClubbyBook`;

INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Художественная литература", NULL, 0, "hudozhestvennaya-literatura"); -- 1
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Наука и техника", NULL, 6, "nauka-i-tehnika"); -- 2
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Гуманитарные науки", NULL, 5, "gumanitarnie-nauki"); -- 3
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Обучение", NULL, 4, "obuchenie"); -- 4
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Искусство", NULL, 1, "iskusstvo"); -- 5
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Медицина. Здоровье.", NULL, 7, "medicina-zdorovje"); -- 6
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Компьютеры", NULL, 2, "kompjyuteri"); -- 7
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Образование", NULL, 3, "obrazovanie"); -- 8 
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Домашнее хозяйство", NULL, 8, "domashnee-hozyajstvo"); -- 9
INSERT INTO `Genre` (`Name`, `ParentId`, `Order`, `UrlRewrite`) VALUES ("Другое", NULL, 9, "drugoe"); -- 10

-- Художественная литература
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Боевики", 1, "boeviki");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Детективы", 1, "detektivi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Детская литература", 1, "detskaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Зарубежная классика", 1, "zarubezhnaya-klassika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Исторические романы", 1, "istoricheskie-romani");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Книги на иностранных языках", 1, "knigi-na-inostrannih-yazikah");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Любовные романы", 1, "lyubovnie-romani");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Русская классика", 1, "russkaya-klassika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Украинская классика", 1, "ukrainskaya-klassika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Современная украинская литература", 1, "sovremennaya-ukrainskaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Поэзия", 1, "poeziya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Приключения", 1, "priklyucheniya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Собрания сочинений", 1, "sobraniya-sochinenij");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Современная зарубежная литература", 1, "sovremennaya-zarubezhnaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Современная русская литература", 1, "sovremennaya-russkaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Триллеры. Ужасы.", 1, "trilleri-uzhasi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Фантастика", 1, "fantastika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Фэнтези", 1, "fentezi");

-- Наука и техника
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Научная литература", 2, "nauchnaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Технические книги", 2, "tehnicheskie-knigi");

-- Гуманитарные науки
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Бизнес.Финансы.", 3, "biznesfinansi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Философская литература", 3, "filosofskaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Юридическая литература", 3, "yuridicheskaya-literatura");

-- Обучение
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Книги для саморазвития", 4, "knigi-dlya-samorazvitiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Энциклопедии. Справочники. Словари.", 4, "enciklopedii-spravochniki-slovari");

-- Искусство
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Архитектура", 5, "arhitektura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Балет, танец", 5, "balet-tanec");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Декоративно-прикладное искусство", 5, "dekorativno-prikladnoe-iskusstvo");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Изобразительное искусство, скульптура", 5, "izobraziteljnoe-iskusstvo-skuljptura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Искусство (другое)", 5, "iskusstvo-drugoe");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Кино, ТВ, видео", 5, "kino-tv-video");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Театр, опера", 5, "teatr-opera");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Теория искусства", 5, "teoriya-iskusstva");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Фотография", 5, "fotografiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Художественные альбомы", 5, "hudozhestvennie-aljbomi");

-- Медицина. Здоровье.
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Детская медицина", 6, "detskaya-medicina");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Здоровый образ жизни", 6, "zdorovij-obraz-zhizni");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Медицина (другое)", 6, "medicina-drugoe");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Медицина для специалистов", 6, "medicina-dlya-specialistov");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Медицинские справочники", 6, "medicinskie-spravochniki");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Нетрадиционная медицина", 6, "netradicionnaya-medicina");

-- Компьютеры
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Графика и дизайн", 7, "grafika-i-dizajn");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Компьютерное железо", 7, "kompjyuternoe-zhelezo");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Компьютерные программы", 7, "kompjyuternie-programmi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Компьютерные сети", 7, "kompjyuternie-seti");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Компьютеры (другое)", 7, "kompjyuteri-drugoe");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Криптография, безопасность", 7, "kriptografiya-bezopasnostj");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Операционные системы", 7, "operacionnie-sistemi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Программирование", 7, "programmirovanie");

-- Образование
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Биология", 8, "biologiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("География", 8, "geografiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Другие школьные предметы", 8, "drugie-shkoljnie-predmeti");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Иностранный язык", 8, "inostrannyj-yazik");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Информатика", 8, "informatika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("История", 8, "istoriya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Математика", 8, "matematika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Медицина", 8, "medicina");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Психология", 8, "psihologiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Техника", 8, "tehnika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Другое", 8, "drugoe-1");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Философия", 8, "filosofiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Химия", 8, "himiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Экология", 8, "ekologiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Экономика, финансы", 8, "ekonomika-finansi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Юриспруденция", 8, "yurisprudenciya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Литература, русский язык", 8, "literatura-russkij-yazik");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Филология", 8, "filologiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Обществзнание", 8, "obschestvznanie");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Пособия для учителей", 8, "posobiya-dlya-uchitelej");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Физика, астрономия", 8, "fizika-astronomiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Химия, экология", 8, "himiya-ekologiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Экономика", 8, "ekonomika");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Методические указания", 8, "metodicheskie-ukazaniya");

-- Домашнее хозяйство
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Домашнее хозяйство (другое)", 9, "domashnee-hozyajstvo-drugoe");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Домашние животные", 9, "domashnie-zhivotnie");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Приготовление пищи", 9, "prigotovlenie-pischi");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Ремонт, строительство", 9, "remont-stroiteljstvo");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Рукоделие", 9, "rukodelie");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Садоводство", 9, "sadovodstvo");

-- Другое
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Антиквариат", 10, "antikvariat");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Биографии. Мемуары.", 10, "biografii-memuari");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Другая зарубежная литература", 10, "drugaya-zarubezhnaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Другая русская литература", 10, "drugaya-russkaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Другая украинская литература", 10, "drugaya-ukrainskaya-literatura");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Журналы", 10, "zhurnali");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Игры", 10, "igri");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Книги о спорте", 10, "knigi-o-sporte");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Коллекционирование", 10, "kollekcionirovanie");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Непознанное. Паранормальное.", 10, "nepoznannoe-paranormaljnoe");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Открытки, конверты", 10, "otkritki-konverti");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Охота, рыбалка", 10, "ohota-ribalka");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Путешествия", 10, "puteshestviya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Религия", 10, "religiya");
INSERT INTO `Genre` (`Name`, `ParentId`, `UrlRewrite`) VALUES ("Увлечения (другое)", 10, "uvlecheniya-drugoe");