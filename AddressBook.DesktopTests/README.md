# Задание №18: группы в Windows Address Book

Отдельный проект UI-тестов на **TestStack.White 0.13.3 + NUnit 3**, созданный по архитектуре из лекции 8.2. Он не зависит от Selenium-проекта и генератора данных.

Самостоятельный репозиторий: [BeautifulFennel/addressbook-tests-white](https://github.com/BeautifulFennel/addressbook-tests-white).

Основное учебное решение: [BeautifulFennel/C-adress-book1](https://github.com/BeautifulFennel/C-adress-book1). Проект можно открыть отдельно через `AddressBook.DesktopTests.sln` или в составе основного решения.

## Структура

```text
AddressBook.DesktopTests/
├── AppManager/
│   ├── ApplicationManager.cs    # запуск, окно приложения, помощники, завершение
│   ├── HelperBase.cs            # общая ссылка помощников на менеджер
│   └── GroupHelper.cs           # чтение, добавление и удаление групп через White
├── Model/
│   └── GroupData.cs             # название группы и сравнение объектов по названию
├── Tests/
│   ├── TestBase.cs              # общие SetUp/TearDown, STA, запрет параллельного запуска
│   ├── GroupCreationTests.cs    # добавление: латиница и кириллица
│   └── GroupRemovalTests.cs     # удаление созданной тестом группы
├── App.config                  # путь к portable-приложению
└── AddressBook.DesktopTests.csproj
```

`TestBase` перед каждым тестом создаёт `ApplicationManager`, а после теста завершает запущенное им приложение. Менеджер предоставляет `MainWindow` и помощник `Groups`. Тесты описывают сценарии и проверки, а все действия с окнами находятся в `GroupHelper`.

`GetGroupList()` открывает редактор групп, читает дочерние узлы корня дерева и закрывает редактор. Корень дерева не считается группой. Это соответствует примеру из урока; тесты создают группы непосредственно в корне.

## Проверки

- `TestGroupCreation("White")` и `TestGroupCreation("Тестовая группа")`: прочитать список, добавить группу, проверить увеличение количества на один и совпадение полного списка с ожидаемым.
- `TestGroupRemoval`: создать собственную группу, прочитать список, удалить именно эту группу, проверить уменьшение количества на один и совпадение полного списка с ожидаемым. Тест не зависит от тестов создания и работает при любом исходном количестве групп.

Имена получают уникальный суффикс. Списки сравниваются через `Is.EquivalentTo`, поэтому порядок сортировки дерева не влияет на результат. `GroupData.Equals()` сравнивает названия; `GetHashCode()` и `CompareTo()` согласованы с этим сравнением.

При удалении выбирается вариант удаления только группы (`uxDeleteGroupsOnlyRadioButton`). Название вводится через `TextBox.BulkText`: в отличие от обычного ввода White этот способ не зависит от раскладки клавиатуры.

## Требования

1. Windows с доступным и разблокированным рабочим столом.
2. .NET Framework 4.8 и его Targeting Pack для сборки.
3. .NET SDK либо Visual Studio с поддержкой .NET Framework.
4. Распакованный AddressBookPortable с `AddressBook.exe`, конфигурацией, MDB и `Interop.ADOX.dll`.

White рассчитан на .NET Framework, поэтому проект использует `net48`. Целевая платформа — `x86`: portable-приложение использует 32-разрядный провайдер Microsoft Jet. NUnit выполняет UI-тесты последовательно в STA. Во время запуска не следует пользоваться мышью и клавиатурой: White управляет ими для работы с приложением.

## Запуск

По умолчанию в `App.config` указан предоставленный путь:

```text
E:\Work work\AI Stuff\C# course\AddressBookPortable\AddressBook.exe
```

Из каталога, в котором находится `AddressBook.DesktopTests.csproj`:

```powershell
dotnet test ".\AddressBook.DesktopTests.csproj"
```

Для другого расположения можно изменить `App.config` или задать переменную окружения, которая имеет приоритет:

```powershell
$env:ADDRESSBOOK_DESKTOP_EXE = 'C:\Tools\AddressBookPortable\AddressBook.exe'
dotnet test ".\AddressBook.DesktopTests.csproj"
```

Отдельный запуск теста удаления:

```powershell
dotnet test ".\AddressBook.DesktopTests.csproj" --filter "FullyQualifiedName~GroupRemovalTests"
```

В Visual Studio откройте `AddressBook.DesktopTests.sln` и запустите тесты в Test Explorer. Пакеты восстановятся через NuGet. В основном репозитории также можно использовать общее решение.

Перед каждым тестом менеджер копирует файлы из каталога приложения в уникальную папку `bin/Debug/net48/TestRuns/<guid>` (либо аналогичную папку выбранной конфигурации). Запускается эта копия; исходная MDB не изменяется. Папки запусков сохраняются для диагностики и исключены из Git общим правилом для `bin/`. Путь к конкретной копии выводится в журнал NUnit.

## Ссылки

- [TestStack.White: исходники](https://github.com/TestStack/White)
- [Пакет White и совместимость с .NET Framework](https://www.nuget.org/packages/TestStack.White/0.13.3)
- [Идентификация элементов White](https://teststackwhite.readthedocs.io/en/latest/UIItems/)
- [Реализация TextBox.Text и BulkText](https://github.com/TestStack/White/blob/master/src/TestStack.White/UIItems/TextBox.cs)
