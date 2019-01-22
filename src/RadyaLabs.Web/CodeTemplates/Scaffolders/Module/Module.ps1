[T4Scaffolding.Scaffolder(Description = "Creates default module controller, service, model, view model and views")][CmdletBinding()]
param(
    [parameter(Mandatory = $true)][String]$Model,
    [parameter(Mandatory = $true)][String]$Controller,
    [parameter(Mandatory = $false)][String]$Area,
    [String[]]$TemplateFolders,
    [Switch]$Delete = $false
)

$DataProject = "RadyaLabs.Data"
$TestsProject = "RadyaLabs.Tests"
$RazorViewProject = "RadyaLabs.Web"
$ObjectsProject = "RadyaLabs.Objects"
$ServiceProject = "RadyaLabs.Services"
$ValidatorProject = "RadyaLabs.Validators"
$ControllerProject = "RadyaLabs.Controllers"

$ElementArea = $Controller
$ElementPath = $Controller
If ($Area) { $ElementArea = "$Area" }
If ($Area) { $ElementPath = "$Area\$Controller" }

$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
. $ScriptDir\ModuleFunctions.ps1

Scaffold-CsTemplate "Tests\ControllerTests" $TestsProject ("Unit\Controllers\$ElementPath\$Controller" + "ControllerTests")
Scaffold-CsTemplate "Tests\ValidatorTests"  $TestsProject ("Unit\Validators\$ElementPath\$Model" + "ValidatorTests")
Scaffold-CsTemplate "Tests\ServiceTests"    $TestsProject ("Unit\Services\$ElementPath\$Model" + "ServiceTests")

Scaffold-CsTemplate "Controls\Model"      $ObjectsProject    "Models\$ElementPath\$Model"
Scaffold-CsTemplate "Controls\View"       $ObjectsProject    ("Views\$ElementPath\$Model" + "View")
Scaffold-CsTemplate "Controls\IValidator" $ValidatorProject  ("$ElementPath\I" + $Model + "Validator")
Scaffold-CsTemplate "Controls\Validator"  $ValidatorProject  ("$ElementPath\$Model" + "Validator")
Scaffold-CsTemplate "Controls\IService"   $ServiceProject    ("$ElementPath\I" + $Model + "Service")
Scaffold-CsTemplate "Controls\Service"    $ServiceProject    ("$ElementPath\$Model" + "Service")
Scaffold-CsTemplate "Controls\Controller" $ControllerProject ("$ElementPath\$Controller" + "Controller")

Scaffold-CshtmlTemplate "Views\Index"   $RazorViewProject "Views\$ElementPath\Index"
Scaffold-CshtmlTemplate "Views\Create"  $RazorViewProject "Views\$ElementPath\Create"
Scaffold-CshtmlTemplate "Views\Details" $RazorViewProject "Views\$ElementPath\Details"
Scaffold-CshtmlTemplate "Views\Edit"    $RazorViewProject "Views\$ElementPath\Edit"
Scaffold-CshtmlTemplate "Views\Delete"  $RazorViewProject "Views\$ElementPath\Delete"

If ($Area)
{
    Scaffold-AreaRegistration "Controls\AreaRegistration"   $ControllerProject ("$ElementArea\$Area" + "AreaRegistration")
    Scaffold-AreaRegistration "Tests\AreaRegistrationTests" $TestsProject ("Unit\Controllers\$ElementArea\$Area" + "AreaRegistrationTests")
}

Scaffold-ObjectCreation $TestsProject "ObjectFactory"
Scaffold-DbSet $DataProject "Context"

If ($Delete)
{
    Delete-EmptyDir $TestsProject "Unit\Validators\$ElementPath"
    Delete-EmptyDir $TestsProject "Unit\Validators\$ElementArea"

    Delete-EmptyDir $TestsProject "Unit\Controllers\$ElementPath"
    if ($Area)
    {
        Delete-AreaRegistration $TestsProject "Unit\Controllers\$ElementArea" ("Unit\Controllers\$ElementArea\$Area" + "AreaRegistrationTests")
    }
    Delete-EmptyDir $TestsProject "Unit\Controllers\$ElementArea"

    Delete-EmptyDir $TestsProject "Unit\Services\$ElementPath"
    Delete-EmptyDir $TestsProject "Unit\Services\$ElementArea"

    Delete-EmptyDir $RazorViewProject "Views\$ElementPath"
    Delete-EmptyDir $RazorViewProject "Views\$ElementArea"

    Delete-EmptyDir $ControllerProject $ElementPath
    If ($Area)
    {
        Delete-AreaRegistration $ControllerProject $ElementArea ("$ElementArea\$Area" + "AreaRegistration")
    }
    Delete-EmptyDir $ControllerProject $ElementArea

    Delete-EmptyDir $ValidatorProject $ElementPath
    Delete-EmptyDir $ValidatorProject $ElementArea

    Delete-EmptyDir $ServiceProject $ElementPath
    Delete-EmptyDir $ServiceProject $ElementArea

    Delete-EmptyDir $ObjectsProject "Views\$ElementPath"
    Delete-EmptyDir $ObjectsProject "Views\$ElementArea"

    Delete-EmptyDir $ObjectsProject "Models\$ElementPath"
    Delete-EmptyDir $ObjectsProject "Models\$ElementArea"
}
