# Задание №20 — проекты в MantisBT

Самостоятельный проект C# / .NET 10 с UI-тестами NUnit и Selenium WebDriver. Его можно открыть отдельно через `Mantis.Tests.sln` или в общем решении `C# adress book1.slnx`. Ссылок на проекты Address Book нет.

## Что реализовано

- `ProjectCreationTests`: создание проекта с латиницей и с кириллицей/спецсимволами. Проверяются увеличение количества, полный список пар `Id/Name` и сохранённые имя/описание на странице редактирования.
- `ProjectRemovalTests`: создание собственного проекта и контрольного проекта как предусловие, удаление целевого проекта с подтверждением, уменьшение количества, отсутствие удалённого `Id` и сохранность остальных проектов, включая контрольный.
- Уникальные имена `hw20-...-GUID` позволяют повторять запуск. `[TearDown]` удаляет только собственные проекты теста, в том числе после падения проверки.
- Тесты выполняются последовательно. Каждый тест получает собственный Firefox; браузер закрывается в `[TearDown]`. При ошибке сохраняется скриншот с вложением в результат NUnit.

## Архитектура

```text
Mantis.Tests/
├── AppManager/
│   ├── ApplicationManager.cs       # браузер, настройки и помощники
│   ├── HelperBase.cs               # ожидания, ввод, клики
│   ├── LoginHelper.cs              # вход и выход
│   ├── ManagementMenuHelper.cs     # Управление → Управление проектами
│   ├── ProjectManagementHelper.cs  # создание, чтение, удаление проектов
│   └── TestConfiguration.cs        # App.config и переменные окружения
├── Model/
│   ├── AccountData.cs
│   └── ProjectData.cs
├── Tests/
│   ├── TestBase.cs                 # подготовка, очистка, закрытие браузера
│   ├── ProjectCreationTests.cs
│   └── ProjectRemovalTests.cs
├── Scripts/Start-Mantis.ps1
├── App.config
├── Mantis.Tests.csproj
└── Mantis.Tests.sln
```

Тесты описывают сценарии и ожидаемый результат. `ApplicationManager` создаёт браузер и связывает помощники. Помощники содержат Selenium-локаторы и шаги работы с MantisBT; общие операции вынесены в `HelperBase`. Модели хранят входные и прочитанные данные. `TestBase` наследуется обоими тестовыми классами и отвечает за жизненный цикл теста.

Локаторы используют имена полей и адреса PHP-форм, поэтому не зависят от русских или английских подписей. Удаление выбирает проект по `Id`, а не по позиции в таблице. Вместо `Thread.Sleep` используются явные ожидания `WebDriverWait`. Данные читаются через UI, доступ к БД из тестов не требуется.

Скриншоты уроков использованы как образец архитектуры. FTP, Telnet, James и POP3 относятся к регистрации пользователей и в задание на добавление/удаление проектов не включены.

## Проверка результата

21.10.2026 выполнены восстановление NuGet-пакетов, сборка отдельного решения и полный UI-прогон на настоящем MantisBT 2.28.4 / PHP 8.2.12 / MariaDB / Firefox 157.0.1 в режиме headless. Сборка завершилась без ошибок и предупреждений; **3 из 3 тестовых случаев прошли**. После прогона в отдельной тестовой базе осталось 0 проектов: очистка выполнена. Отчёт последнего полного прогона — `TestResults/mantis.trx` (локальный файл, исключён из Git).

## Запуск на этом компьютере

Подготовлен локальный стенд MantisBT 2.28.4: исходники и зависимости находятся в игнорируемом Git каталоге `../artifacts/mantis/source/`, база MariaDB — `mantis_hw20`. Используется установленный XAMPP: `C:\Program Files\XAMPP`. Вход в учебный MantisBT: `administrator` / `root`.

Команды выполняются из корня репозитория:

1. Если после перезапуска компьютера MariaDB остановлена, откройте XAMPP Control Panel и нажмите **Start** напротив **MySQL**.
2. Запустите локальный веб-сервер:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File ".\Mantis.Tests\Scripts\Start-Mantis.ps1"
   ```

   Откройте `http://127.0.0.1:8085/`. Скрипт использует PHP из XAMPP, запускает сервер в фоне и записывает PID/логи в `Mantis.Tests/.local/`. Apache для этого адреса не требуется.

3. Запустите тесты:

   ```powershell
   dotnet test ".\Mantis.Tests\Mantis.Tests.csproj"
   ```

   Для запуска без видимого окна Firefox:

   ```powershell
   $env:MANTIS_HEADLESS = "true"
   dotnet test ".\Mantis.Tests\Mantis.Tests.csproj" --logger "trx;LogFileName=mantis.trx" --results-directory ".\Mantis.Tests\TestResults"
   ```

В Visual Studio откройте решение и выберите **Test → Test Explorer → Run All Tests** для проекта `Mantis.Tests`. Должны обнаружиться три тестовых случая: два создания и один удаления.

## Настройки и другой стенд

В `App.config` задаётся адрес **каталога** MantisBT, с завершающим `/`, без `login_page.php`. Для другого стенда измените файл или задайте переменные окружения перед запуском:

| Параметр App.config | Переменная окружения | Значение по умолчанию |
| --- | --- | --- |
| `BaseUrl` | `MANTIS_BASE_URL` | `http://127.0.0.1:8085/` |
| `Username` | `MANTIS_USERNAME` | `administrator` |
| `Password` | `MANTIS_PASSWORD` | `root` |
| `Headless` | `MANTIS_HEADLESS` | `false` |
| `TimeoutSeconds` | `MANTIS_TIMEOUT_SECONDS` | `15` |

Например, для MantisBT в `htdocs/mantisbt`:

```powershell
$env:MANTIS_BASE_URL = "http://localhost/mantisbt/"
$env:MANTIS_USERNAME = "administrator"
$env:MANTIS_PASSWORD = "ваш пароль"
dotnet test ".\Mantis.Tests\Mantis.Tests.csproj"
```

Переменные имеют приоритет над файлом; в PowerShell они действуют в текущем окне. У пользователя должны быть права создавать и удалять проекты. Если используется существующий стенд через Apache, запускать `Start-Mantis.ps1` не нужно.

## Установка на другом компьютере

1. Установите .NET 10 SDK, Firefox и XAMPP с PHP 7.4/8.x и MariaDB. NuGet-зависимости восстановятся при `dotnet test`. Selenium Manager подберёт geckodriver; для первого получения драйвера нужен интернет. Если драйвер уже находится в `PATH`, используется он.
2. Скачайте полный дистрибутив MantisBT со [страницы загрузки](https://www.mantisbt.org/download.php) и распакуйте, например, в `C:\xampp\htdocs\mantisbt`. Полный дистрибутив содержит зависимости PHP; архив исходников GitHub требует отдельного `composer install --no-dev`.
3. Запустите Apache и MySQL в XAMPP. Откройте `http://localhost/mantisbt/admin/install.php`.
4. Укажите тип `MySQL Improved`, сервер `localhost`, отдельную базу `mantis_hw20`, учётные данные вашей MariaDB в полях Database и Admin. В стандартном локальном XAMPP это `root` и пустой пароль. Нажмите **Install/Upgrade Database**, дождитесь успешного завершения.
5. Войдите в MantisBT как `administrator` с первоначальным паролем `root`. Укажите адрес и фактические учётные данные в настройках тестов, затем выполните команду запуска выше.

Если нужен встроенный PHP-сервер вместо Apache:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File ".\Mantis.Tests\Scripts\Start-Mantis.ps1" -PhpPath "C:\xampp\php\php.exe" -MantisPath "C:\xampp\htdocs\mantisbt"
```

Исходный код приложения: [официальный MantisBT](https://github.com/mantisbt/mantisbt). Управление браузером и драйвером: [документация Selenium Manager](https://www.selenium.dev/documentation/selenium_manager/).
