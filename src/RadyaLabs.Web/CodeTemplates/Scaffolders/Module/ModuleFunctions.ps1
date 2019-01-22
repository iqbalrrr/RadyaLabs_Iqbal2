Function Scaffold-CsTemplate([String]$Template, [String]$Project, [String]$OutputPath)
{
    if ($Delete)
    {
        Delete-ProjectItem $Project "$OutputPath.cs"
        Return;
    }

    $ShortModel = ([Regex] "(?=[A-Z])").Split($Model, 0, 0)[-1];
    $ModelVarName = $ShortModel.SubString(0, 1).ToLower() + $ShortModel.SubString(1)

    $ControllerNamespace = "RadyaLabs.Controllers"
    $ControllerTestsNamespace = "RadyaLabs.Tests.Unit.Controllers"
    If ($Area) { $ControllerNamespace = "RadyaLabs.Controllers.$Area" }
    If ($Area) { $ControllerTestsNamespace = "RadyaLabs.Tests.Unit.Controllers.$Area" }

    Add-ProjectItemViaTemplate `
        -OutputPath $OutputPath `
        -Template $Template `
        -Model @{ `
            ControllerTestsNamespace = $ControllerTestsNamespace; `
            AreaRegistration = $Area + "AreaRegistration"; `
            ControllerNamespace = $ControllerNamespace; `
            ModelVarName = $ModelVarName; `

            Controller = $Controller + "Controller"; `
            IValidator = "I" + $Model + "Validator"; `
            IService = "I" + $Model + "Service"; `
            Validator = $Model + "Validator"; `
            Service = $Model + "Service"; `
            ShortModel = $ShortModel; `
            View = $Model + "View"; `
            Model = $Model; `
            Area = $Area; `
        } `
        -SuccessMessage "Added $Project\{0}." `
        -TemplateFolders $TemplateFolders `
        -Project $Project
}
Function Scaffold-AreaRegistration([String]$Template, [String]$Project, [String]$OutputPath)
{
    if (!$Delete)
    {
        $ControllerNamespace = "RadyaLabs.Controllers.$Area"
        $ControllerTestsNamespace = "RadyaLabs.Tests.Unit.Controllers.$Area"

        Add-ProjectItemViaTemplate `
            -OutputPath $OutputPath `
            -Template $Template `
            -Model @{ `
                ControllerTestsNamespace = $ControllerTestsNamespace; `
                AreaRegistration = $Area + "AreaRegistration"; `
                ControllerNamespace = $ControllerNamespace; `
                Area = $Area; `
            } `
            -SuccessMessage "Added $Project\{0}." `
            -TemplateFolders $TemplateFolders `
            -Project $Project
    }
}
Function Scaffold-CshtmlTemplate([String]$Template, [String]$Project, [String]$OutputPath)
{
    if ($Delete)
    {
        Delete-ProjectItem $Project "$OutputPath.cshtml"
        Return;
    }

	$Models = Get-PluralizedWord $Model

    Add-ProjectItemViaTemplate `
        -OutputPath $OutputPath `
        -Template $Template `
        -Model @{ `
            View = $Model + "View"; `
            Models = $Models; `
            Model = $Model; `
        } `
        -SuccessMessage "Added $Project\{0}." `
        -TemplateFolders $TemplateFolders `
        -Project $Project
}

Function Scaffold-ObjectCreation([String]$Project, [String]$Factory)
{
    if (!$Delete)
    {
        $FactoryClass = Get-ProjectType -Project $Project -Type $Factory

        Add-ClassMemberViaTemplate `
            -SuccessMessage "Added tests object creation functions to $Factory." `
            -Template "Members\ObjectCreation" `
            -TemplateFolders $TemplateFolders `
            -CodeClass $FactoryClass `
            -Model @{ `
                View = $Model + "View"; `
                Model = $Model; `
            }
    }
}

Function Scaffold-DbSet([String]$Project, [String]$Context)
{
    if (!$Delete)
    {
        $ContextClass = Get-ProjectType -Project $Project -Type $Context
        $Models = Get-PluralizedWord $Model

        Add-ClassMemberViaTemplate `
            -SuccessMessage "Added DbSet<$Model> member to $Context." `
            -TemplateFolders $TemplateFolders `
            -Template "Members\DbSet" `
            -CodeClass $ContextClass `
            -Model @{ `
                Models = $Models; `
                Model = $Model; `
            }
    }
}

Function Delete-AreaRegistration([String]$Project, [String]$AreaPath, [String]$Path)
{
    $AreaRegistrationDir = Get-ProjectItem -Project $Project -Path $AreaPath
    If ($AreaRegistrationDir)
    {
        $ObjectCount = (Get-ChildItem $AreaRegistrationDir.FileNames(0) | Measure-Object).Count
        If ($ObjectCount -eq 1)
        {
            Delete-ProjectItem $Project "$Path.cs"
            Delete-EmptyDir $Project $AreaPath
        }
    }
}
Function Delete-ProjectItem([String]$Project, [String]$Path)
{
    $ProjectItem = Get-ProjectItem -Project $Project -Path $Path
    If ($ProjectItem)
    {
        $ProjectItem.Delete()
        Write-Host "Deleted $Project\$Path"
    }
    Else
    {
        Write-Host "$Project\$Path is missing! Skipping..." -BackgroundColor Yellow;
    }
}

Function Delete-EmptyDir([String]$Project, [String]$Dir)
{
    $ProjectItem = Get-ProjectItem -Project $Project -Path $Dir
    If ($ProjectItem -and !(Get-ChildItem $ProjectItem.FileNames(0) -force))
    {
        $ProjectItem.Delete()
        Write-Host "Deleted $Project\$Dir"
    }
}
