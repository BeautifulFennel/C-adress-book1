# C# Address Book — UI-автотесты

Учебное решение с UI-автотестами для Address Book. Веб-тесты запускают Firefox через Selenium WebDriver и проверяют авторизацию, а также основные операции с группами и контактами. Отдельный проект `AddressBook.DesktopTests` проверяет группы в Windows-приложении через White.

## Задание №18 — Windows Address Book

Создан самостоятельный проект [AddressBook.DesktopTests](AddressBook.DesktopTests/README.md) на .NET Framework 4.8, TestStack.White и NUnit. В нём есть тесты добавления и удаления групп, `TestBase`, `ApplicationManager`, `HelperBase`, `GroupHelper` и модель `GroupData`. Каждый тест работает с собственной копией portable-приложения и MDB. Проверяются количество и полный состав групп.

Отдельный репозиторий задания: [BeautifulFennel/addressbook-tests-white](https://github.com/BeautifulFennel/addressbook-tests-white).

```powershell
dotnet test ".\AddressBook.DesktopTests\AddressBook.DesktopTests.csproj"
```

Требуется Windows с разблокированным рабочим столом. Настройка пути, архитектура и подробности запуска описаны в README проекта по ссылке выше. Остальные разделы ниже относятся к веб-тестам.

## Текущее состояние

В проекте реализованы следующие сценарии:

- вход с корректными и некорректными учётными данными;
- создание обычной и пустой группы;
- изменение и удаление группы, выбранной из БД;
- создание контакта, изменение и удаление контакта, выбранного из БД;
- добавление контакта в группу и исключение контакта из группы с проверкой состава группы по БД;
- автоматическое создание группы или контакта, если для изменения либо удаления нет подходящих данных;
- учебный unit-тест, демонстрирующий сравнение ссылок на объекты.

Архитектура тестов построена вокруг `ApplicationManager`. Он управляет экземпляром `FirefoxDriver` и предоставляет специализированные помощники для авторизации, навигации, групп и контактов. Общая подготовка вынесена в `TestBase`, а авторизация — в `AuthTestBase`.

В задании №16 списки до и после операций загружаются из БД через `GroupData.GetAll()` и `ContactData.GetAll()`. Создание, изменение и удаление выполняются через Selenium; запись для изменения или удаления выбирается по `Id` из БД. XML/JSON остаются источниками входных данных тестов.

`GroupTestBase` и `ContactTestBase` добавляют отключаемую сверку UI с БД после каждого соответствующего UI-теста. Unit-тесты моделей и сериализации запускаются независимо от браузера и БД.

## Технологии

- C# и .NET 10;
- NUnit 3;
- Selenium WebDriver 4;
- LINQ to DB и MySqlConnector для чтения MySQL/MariaDB;
- Firefox;
- Visual Studio или .NET CLI.

## Требования

Перед запуском необходимы:

1. .NET SDK 10;
2. установленный Firefox;
3. доступный Firefox WebDriver (настройка может выполняться Selenium Manager автоматически);
4. запущенное локально приложение Address Book по адресу `http://localhost/addressbook/`;
5. тестовая учётная запись `admin` с паролем `secret`;
6. доступная MySQL/MariaDB с базой `addressbook`, которую использует это же приложение.

Тесты изменяют данные в Address Book: создают, редактируют и удаляют группы и контакты. Их следует запускать только на тестовом экземпляре приложения.

## Запуск

Параметры БД находятся в `C# adress book1/App.config`, в подключении с именем `AddressBook`. По умолчанию указаны `localhost:3306`, база `addressbook`, пользователь `root` и пустой пароль, как в лекции. Для другого окружения строку можно переопределить переменной `ADDRESSBOOK_CONNECTION_STRING`.

Класс `AdressBookDB : DataConnection` предоставляет таблицы `Groups`, `Contacts` и `GCR`. Атрибуты `[Table]` и `[Column]` связывают модели с таблицами и столбцами. `GroupData.GetContacts()` получает контакты группы через `address_in_groups`. `ContactData.GetAll()` и `GroupData.GetContacts()` считают активными записи с `deprecated = '0000-00-00 00:00:00'` либо `NULL`. Записи с датой удаления, включая старые версии изменённых контактов, исключаются на стороне БД. Поле `Deprecated` читается как строка, поскольку нулевая дата не представима в `DateTime`; в строке подключения включено `Allow Zero Datetime=true`. При переопределении `ADDRESSBOOK_CONNECTION_STRING` этот параметр тоже следует указать. См. [параметры MySqlConnector](https://mysqlconnector.net/connection-options/). Соединения закрываются через `using`.

В .NET 10 настройки из `App.config` явно читает `TestConfiguration` и передаёт их в `DataOptions`. Это адаптация примера лекции, написанного для .NET Framework. Используются пакеты `linq2db` и `MySqlConnector` с nuget.org; современный `linq2db.MySql` содержит шаблоны генерации моделей, которые здесь не нужны: [описание пакета](https://www.nuget.org/packages/linq2db.MySql).

Из корневого каталога репозитория выполните:

```powershell
dotnet restore ".\C# adress book1\C# adress book1.csproj"
dotnet test ".\C# adress book1\C# adress book1.csproj"
```

Для запуска отдельного набора тестов можно использовать фильтр, например:

```powershell
dotnet test ".\C# adress book1\C# adress book1.csproj" --filter "FullyQualifiedName~LoginTests"
```

Только тесты задания №17:

```powershell
dotnet test ".\C# adress book1\C# adress book1.csproj" --filter "FullyQualifiedName~AddingContactToGroupTests|FullyQualifiedName~RemovingContactFromGroupTests"
```

Эти тесты подготавливают недостающие группы, контакты и членство через UI. Контакт и группа выбираются по `Id`, поэтому одинаковые имена и пустые названия групп не мешают проверке. После операции тест заново читает полный состав группы из БД и сравнивает его с ожидаемым. Общий список контактов также проверяется: добавление или исключение из группы не должно менять сами контакты.

Для исключения используется кнопка `Remove from "..."` (`name="remove"`). Кнопка `Delete` (`name="delete"`) удаляет контакт целиком и проверяется отдельно в `ContactRemovalTests`. Обе операции с группой дожидаются видимого блока `div.msgbox` перед проверкой БД.

Дополнительная сверка UI и БД по умолчанию выключена. Для включения установите в `App.config`:

```xml
<add key="PERFORM_LONG_UI_CHECKS" value="true" />
```

Или задайте переменную только для текущего запуска PowerShell:

```powershell
$env:PERFORM_LONG_UI_CHECKS = "true"
dotnet test ".\C# adress book1\C# adress book1.csproj"
Remove-Item Env:PERFORM_LONG_UI_CHECKS
```

Переменная окружения имеет приоритет над файлом. При включённом флаге `[TearDown]` обновляет страницу, заново читает список с обходом кеша и сравнивает его с БД. Для групп сравниваются `Id` и название; для контактов — `Id`, имя, фамилия, адрес, телефоны и e-mail. Сравнение выполняется по `Id`, поэтому порядок строк не влияет на результат. Для выключения задайте `false` или верните настройку файла в `false` и удалите переменную окружения.

Если каталог `obj` занят Visual Studio или недоступен для записи, можно выполнить сборку и тесты в отдельном каталоге:

```powershell
dotnet test ".\C# adress book1\C# adress book1.csproj" --artifacts-path ".\artifacts\hw16"
```

## Структура проекта

```text
C# adress book1/
├── Appmaganer/    # ApplicationManager и помощники Selenium
├── Model/         # модели учётной записи, группы и контакта
├── Tests/         # UI-автотесты NUnit
├── UnitTest1.cs   # учебный unit-тест
└── C# adress book1.csproj
```

## История версий

Отдельные Git-теги в репозитории пока не используются, поэтому версии ниже соответствуют учебным выкладкам-коммитам. Новые записи следует добавлять в начало раздела.

### 2026-10-19 — Homework №18

- добавлен отдельный проект `AddressBook.DesktopTests` на White и NUnit с типовой архитектурой тестового набора;
- реализованы добавление групп с латиницей и кириллицей и независимый тест удаления собственной группы;
- проверяются изменение количества и полный список названий; ввод через `BulkText` не зависит от раскладки;
- приложение и MDB копируются для каждого теста, запуск выполняется последовательно в STA, платформа — x86.

Проверка: сборка проекта успешна без предупреждений; все 3 UI-теста прошли на предоставленном Windows-приложении.

### 2026-10-15 — Homework №17

- добавлены `AddingContactToGroupTests` и `RemovingContactFromGroupTests`: подготовка данных через UI, изменение членства и проверка полного состава группы по БД;
- проверяется сохранность общего списка контактов после обеих операций;
- в `ContactHelper` добавлены выбор группы по `Id`, сброс фильтра, ожидание `div.msgbox` и прокрутка к выпадающим спискам;
- `ContactData.GetAll()` и `GroupData.GetContacts()` поддерживают активные записи с нулевой датой и `NULL` в `deprecated`, исключая удалённые и старые версии изменённых контактов;
- в подключении включено `Allow Zero Datetime=true`, поле `Deprecated` читается как строка.
- существующий тест страницы свойств учитывает блок `Member of` для контактов, состоящих в группах.

Проверка: 74/74 теста прошли со включённой сверкой UI и БД. Фильтрация `deprecated` отдельно проверена на временных записях с `NULL`, нулевой датой и датой удаления; временные записи удалены. Сборка успешна; при восстановлении пакетов получено предупреждение `NU1900` из-за недоступности сервиса аудита NuGet.

### 2026-10-15 — Homework №16

- добавлены LINQ to DB, MySqlConnector и класс `AdressBookDB` для чтения MySQL/MariaDB;
- списки групп и контактов в тестах загружаются из БД; операции в UI выполняются по `Id`;
- добавлены модель `GroupContactRelation` и получение контактов группы через `GetContacts()`;
- добавлены отключаемые проверки UI и БД в `[TearDown]`, управляемые `PERFORM_LONG_UI_CHECKS`;
- добавлена прокрутка к выбранному элементу для работы с длинными и широкими таблицами.

Проверка: сборка решения без ошибок и предупреждений; 72/72 теста пройдены с выключенной и включённой дополнительной сверкой. Во включённом режиме выполнено 27 сверок UI и БД. Связь группы и контакта проверена через интерфейс.

### 2026-09-17 — Homework №8

- добавлены модель `AccountData` и отдельная базовая фикстура `AuthTestBase`;
- добавлены позитивный и негативный тесты авторизации;
- реализованы проверки текущей сессии и защита от повторного входа, выхода и перехода на уже открытую страницу;
- `ApplicationManager` переведён на один общий экземпляр для тестов.

Коммит: [`c8a206b`](https://github.com/BeautifulFennel/C-adress-book1/commit/c8a206b152e516d90d4f9f3c40e5cccb6c4f2d0d)

### 2026-09-15 — Homework №7

- добавлены тесты изменения и удаления контактов и групп;
- добавлена подготовка недостающих тестовых данных;
- помощники получили операции изменения, удаления и проверки наличия записей.

Коммит: [`8ee6884`](https://github.com/BeautifulFennel/C-adress-book1/commit/8ee6884d85712211bdf21663b9315c6d1e052771)

### 2026-09-11 — доработка Homework №6

- общая подготовка браузера и авторизация перенесены в `TestBase`;
- низкоуровневые шаги объединены в составные операции `Create` и `Remove`;
- добавлен тест создания пустой группы;
- помощники переведены на fluent interface и работу через `ApplicationManager`.

Коммит: [`6cb1bb0`](https://github.com/BeautifulFennel/C-adress-book1/commit/6cb1bb09121a5f0c5b33098206c01c0f51b5ae0e)

### 2026-09-09 — Homework №6

- тестовый код разделён на менеджер приложения, помощники, модели и тестовые классы;
- добавлены `ApplicationManager`, `LoginHelper`, `NavigationHelper`, `GroupHelper` и `ContactHelper`;
- добавлен тест удаления группы;
- устранено дублирование Selenium-шагов в тестах.

Коммит: [`58c3524`](https://github.com/BeautifulFennel/C-adress-book1/commit/58c352475fc5bdfadd7b75c614950eb150a9e8b4)

### 2026-09-02 — Homework №4

- добавлен Selenium-тест создания контакта;
- добавлена модель `ContactData` для передачи данных контакта в тестовые шаги.

Коммит: [`f8f7402`](https://github.com/BeautifulFennel/C-adress-book1/commit/f8f7402422201dace90572811df7823df4d863cf)

### 2026-09-02 — Task №3

- добавлена модель `GroupData`;
- сценарий создания группы разбит на понятные вспомогательные методы;
- данные группы отделены от логики работы с браузером;
- добавлен учебный NUnit-тест сравнения ссылок на объекты.

Коммит: [`4a2028a`](https://github.com/BeautifulFennel/C-adress-book1/commit/4a2028a304dbd69567e43296eb0eedca8f59dbef)

### 2026-08-31 — Homework №2

- подключены NUnit и Selenium WebDriver;
- добавлен и приведён к рабочему сценарию первый UI-тест;
- реализовано создание группы с входом в приложение и последующим выходом.

Коммиты: [`7e8a412`](https://github.com/BeautifulFennel/C-adress-book1/commit/7e8a412bbb3322b6e837e4b5ec92a6c372c9f879), [`2e857b2`](https://github.com/BeautifulFennel/C-adress-book1/commit/2e857b21fd418feba8ce77469ce45e7875ab69dc), [`aa0ca42`](https://github.com/BeautifulFennel/C-adress-book1/commit/aa0ca42efbd588f4bef730b14f582389bcccb6bc)

### 2026-08-29 — создание проекта

- создано решение и тестовый проект;
- добавлены базовые файлы репозитория и начальный unit-тест.

Коммиты: [`bb4ea76`](https://github.com/BeautifulFennel/C-adress-book1/commit/bb4ea767c6c4bcc2b50255c49bb699055af46e78), [`ad3b346`](https://github.com/BeautifulFennel/C-adress-book1/commit/ad3b34620880dd49739df5064d36ece39b65738d), [`d168cd7`](https://github.com/BeautifulFennel/C-adress-book1/commit/d168cd74fa85fa0dee9925c82eccbb590967e461)
