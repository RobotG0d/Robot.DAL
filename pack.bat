.\nuget.exe pack .\Robot.Expected\ -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack .\Robot.DAL.Core\ -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack .\Robot.DAL.Orchestration\ -Properties Configuration=Release -IncludeReferencedProjects
.\nuget.exe pack .\Robot.DAL.SqlServer\ -Properties Configuration=Release -IncludeReferencedProjects
pause