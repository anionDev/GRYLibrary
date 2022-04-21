import os
from pathlib import Path
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore

def run_testcases_for_csharp_project(sc:ScriptCollectionCore,repository_folder:str):
    testargument = f"test GRYLibraryTests/GRYLibraryTests.csproj -c QualityCheck" \
        f" --verbosity normal /p:CollectCoverage=true /p:CoverletOutput=/Other/TestCoverage/TestCoverage.xml" \
        f" /p:CoverletOutputFormat=opencover"
    sc.start_program_synchronously("dotnet", testargument, repository_folder)

def RunTestcasesStarter():
    sc=ScriptCollectionCore()
    repository_folder=str(Path(os.path.dirname(__file__)).parent.absolute())
    run_testcases_for_csharp_project(sc,repository_folder)
    sc.generate_coverage_report(repository_folder)

if __name__=="__main__":
    RunTestcasesStarter()
