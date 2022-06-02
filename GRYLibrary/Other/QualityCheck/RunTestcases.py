import os
import sys
from pathlib import Path
from lxml import etree
from ScriptCollection.ScriptCollectionCore import ScriptCollectionCore
from ScriptCollection.GeneralUtilities import GeneralUtilities


@GeneralUtilities.check_arguments
def generate_coverage_report(self: ScriptCollectionCore, repository_folder: str, codeunitname: str, verbosity: int):
    """This script expects that the file '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/TestCoverage.xml' which contains a test-coverage-report in the cobertura-format exists.
This script expectes that the testcoverage-reportfolder is '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/TestCoverageReport'.
This script expectes that a test-coverage-badges should be added to '<repositorybasefolder>/<codeunitname>/Other/QualityCheck/TestCoverage/Badges'."""
    if verbosity == 0:
        verbose_argument_for_reportgenerator = "Off"
    if verbosity == 1:
        verbose_argument_for_reportgenerator = "Error"
    if verbosity == 2:
        verbose_argument_for_reportgenerator = "Info"
    if verbosity == 3:
        verbose_argument_for_reportgenerator = "Verbose"

    # Generating report
    GeneralUtilities.ensure_directory_does_not_exist(os.path.join(repository_folder, codeunitname, "Other/QualityCheck/TestCoverage/TestCoverageReport"))
    GeneralUtilities.ensure_directory_exists(os.path.join(repository_folder, codeunitname, "Other/QualityCheck/TestCoverage/TestCoverageReport"))
    self.run_program("reportgenerator", "-reports:Other/QualityCheck/TestCoverage/TestCoverage.xml -targetdir:Other/QualityCheck/TestCoverage/TestCoverageReport " +
                     f"-verbosity:{verbose_argument_for_reportgenerator}", os.path.join(repository_folder, codeunitname))

    # Generating badges
    GeneralUtilities.ensure_directory_does_not_exist(os.path.join(repository_folder, codeunitname,"Other/QualityCheck/TestCoverage/Badges"))
    GeneralUtilities.ensure_directory_exists(os.path.join(repository_folder, codeunitname,"Other/QualityCheck/TestCoverage/Badges"))
    self.run_program("reportgenerator", "-reports:Other/QualityCheck/TestCoverage/TestCoverage.xml -targetdir:Other/QualityCheck/TestCoverage/Badges -reporttypes:Badges " +
                     f"-verbosity:{verbose_argument_for_reportgenerator}",  os.path.join(repository_folder, codeunitname))


@GeneralUtilities.check_arguments
def standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(self: ScriptCollectionCore, runtestcases_file: str,buildconfiguration: str = "Release", commandline_arguments: list[str] = []):
    repository_folder: str = str(Path(os.path.dirname(runtestcases_file)).parent.parent.parent.absolute())
    codeunit_name: str = os.path.basename(str(Path(os.path.dirname(runtestcases_file)).parent.parent.absolute()))
    for commandline_argument in commandline_arguments:
        if commandline_argument.startswith("-buildconfiguration:"):
            buildconfiguration = commandline_argument[len("-buildconfiguration:"):]
    testprojectname = codeunit_name+"Tests"
    coveragefilesource = os.path.join(repository_folder, codeunit_name, testprojectname, "TestCoverage.xml")
    coveragefiletarget = os.path.join(repository_folder, codeunit_name, "Other/QualityCheck/TestCoverage/TestCoverage.xml")
    GeneralUtilities.ensure_file_does_not_exist(coveragefilesource)
    self.run_program("dotnet", f"test {testprojectname}/{testprojectname}.csproj -c {buildconfiguration}"
                     f" --verbosity normal /p:CollectCoverage=true /p:CoverletOutput=TestCoverage.xml"
                     f" /p:CoverletOutputFormat=cobertura", os.path.join(repository_folder, codeunit_name))
    GeneralUtilities.ensure_file_does_not_exist(coveragefiletarget)
    os.rename(coveragefilesource, coveragefiletarget)
    generate_coverage_report(ScriptCollectionCore(), repository_folder, codeunit_name, 1)


def run_testcases():
    standardized_tasks_run_testcases_for_dotnet_project_in_common_project_structure(ScriptCollectionCore(), str(Path(__file__).absolute()), "QualityCheck", sys.argv)


if __name__ == "__main__":
    run_testcases()
