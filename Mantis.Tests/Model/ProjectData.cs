namespace mantis_tests;

// Id появляется после сохранения; имя и описание задаются перед созданием.
public sealed record ProjectData(string Name, string Description = "")
{
    public int? Id { get; init; }
}
