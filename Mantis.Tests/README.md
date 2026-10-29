# Задания №20 и №22 — проекты в MantisBT и MantisConnect

Самостоятельный проект C# / .NET 10 с UI-тестами NUnit и Selenium WebDriver. Его можно открыть отдельно через `Mantis.Tests.sln` или в общем решении `C# adress book1.slnx`. Ссылок на проекты Address Book нет.

## Что реализовано

- `ProjectCreationTests`: создание проекта с латиницей и с кириллицей/спецсимволами. Проверяются увеличение количества, полный список пар `Id/Name` и сохранённые имя/описание на странице редактирования.
- `ProjectRemovalTests`: получение списка через SOAP и выбор существующего проекта. Только если список пуст, предусловие создаёт проект через `mc_project_add` и повторно читает список. Удаление выполняется через UI с подтверждением; проверяются уменьшение количества, отсутствие удалённого `Id` и сохранность всех остальных проектов.
- `APIHelper`: операции MantisConnect `mc_projects_get_user_accessible` и `mc_project_add`. Списки до/после UI-операций и при очистке получаются через SOAP; возвращаются `Id`, имя и описание. Ошибки SOAP Fault выводятся с названием операции и сообщением сервера.
- Уникальные имена `hw20-...-GUID` позволяют повторять запуск. `[TearDown]` удаляет только собственные проекты теста, в том числе после падения проверки.
- Тесты выполняются последовательно. Каждый тест получает собственный Firefox; браузер закрывается в `[TearDown]`. При ошибке сохраняется скриншот с вложением в результат NUnit.

## Архитектура

```text
Mantis.Tests/
├── AppManager/
│   ├── ApplicationManager.cs       # браузер, настройки и помощники
│   ├── APIHelper.cs                # список и создание через MantisConnect
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

Локаторы используют имена полей и адреса PHP-форм, поэтому не зависят от русских или английских подписей. Удаление выбирает проект по `Id`, а не по позиции в таблице. Вместо `Thread.Sleep` используются явные ожидания `WebDriverWait`. `app.Projects.GetProjectList()` делегирует чтение в `app.API.GetProjectList()`, не открывая страницы браузера. Проверяемые создание и удаление остаются UI-операциями; подготовка при пустом списке выполняется через `app.API.CreateProject(...)`. Доступ к БД из тестов не требуется.

В лекции используется сгенерированный клиент .NET Framework. Здесь для .NET 10 реализованы две нужные операции SOAP 1.1 RPC/encoded через стандартные `HttpClient` и `XDocument`, без дополнительных NuGet-пакетов. `XDocument` экранирует спецсимволы в имени, описании и учётных данных. Контракт сверен с WSDL локального MantisBT.

Скриншоты уроков использованы как образец архитектуры. FTP, Telnet, James и POP3 относятся к регистрации пользователей и в задание на добавление/удаление проектов не включены.

## Проверка результата

29.10.2026 выполнен полный UI/SOAP-прогон на локальном MantisBT 2.28.4 / PHP 8.2.12 / MariaDB / Firefox в режиме headless: **3 из 3 тестовых случаев прошли**. Тест удаления прошёл с изначально пустым списком, создав предусловие через SOAP. Отчёт — `TestResults/mantis-hw22.trx` (локальный файл, исключён из Git).

Отдельный прогон удаления при двух заранее созданных через SOAP проектах также прошёл: удалён выбранный проект, второй сохранён (`TestResults/mantis-hw22-existing.trx`). Дополнительно проверены SOAP-создание/чтение с кириллицей и XML-спецсимволами и сообщение `SOAP Fault: Access denied` при неправильном пароле. Сборка отдельного решения завершилась без ошибок и предупреждений. После очистки проверочных данных в учебной базе осталось 0 проектов.

## Запуск на этом компьютере

Подготовлен локальный стенд MantisBT 2.28.4: исходники и зависимости находятся в игнорируемом Git каталоге `../artifacts/mantis/source/`, база MariaDB — `mantis_hw20`. Используется установленный XAMPP: `C:\Program Files\XAMPP`. Вход в учебный MantisBT: `administrator` / `root`.

Команды выполняются из корня репозитория:

1. Если после перезапуска компьютера MariaDB остановлена, откройте XAMPP Control Panel и нажмите **Start** напротив **MySQL**.
2. Запустите локальный веб-сервер:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File ".\Mantis.Tests\Scripts\Start-Mantis.ps1"
   ```

   Откройте `http://127.0.0.1:8085/`. Скрипт использует PHP из XAMPP, запускает сервер в фоне и записывает PID/логи в `Mantis.Tests/.local/`. Если SOAP не включён в `php.ini`, скрипт подключает расширение параметром `-d extension=soap` для своего процесса. Apache для этого адреса не требуется.

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

Тест удаления выбирает существующий доступный проект, поэтому запускайте его на учебном стенде с данными, которые можно удалять.

## SOAP в PHP

WSDL доступен по адресу `<BaseUrl>api/soap/mantisconnect.php?wsdl`, на подготовленном стенде — `http://127.0.0.1:8085/api/soap/mantisconnect.php?wsdl`. Адрес SOAP-сервиса строится из того же `BaseUrl`, логин и пароль берутся из тех же настроек, что для UI.

Для Apache откройте используемый им `php.ini` (например, `C:\xampp\php\php.ini`), найдите `;extension=soap`, `;extension=php_soap` или `;extension=php_soap.dll` и удалите `;` в начале соответствующей строки. Затем перезапустите Apache через XAMPP Control Panel. Если расширение уже включено, изменение не требуется. Для `Start-Mantis.ps1` оно подключается автоматически при наличии DLL расширения в PHP.

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
