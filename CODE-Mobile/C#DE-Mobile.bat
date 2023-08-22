:: ///////////////////////////////////////////////////
:: C#DE Mobile
:: This batch script is part of the C#DE Mobile App
:: https://github.com/fadilfadz01/CODE-Mobile
:: Copyright (c) Fadil Fadz - @fadilfadz01
:: ///////////////////////////////////////////////////

@echo off
set cSharpFile=%1
powershell.exe -Command "$successCode = 'using System; public class Success{ public static void Main(){ Console.WriteLine(\"Excecuted successfully.\"); } }'; $failedCode = 'using System; public class Failed{ public static void Main(){ Console.WriteLine(\"Excecuted unsuccessfully.\"); } }'; $csharpCode = Get-Content -Path '%cSharpFile%' -Raw; try{ Add-Type -TypeDefinition $csharpCode -Language CSharp -ErrorAction Stop; [Program]::Main(); Add-Type -TypeDefinition $successCode -Language CSharp -ErrorAction Stop; [Success]::Main() } catch{ if($_.TargetObject.ErrorText -eq $null){ (\"Error:`n At`n  {0}\" -f $_.Exception.message) } else{ (\"Error:`n At`n  Line {0}`n   {1}\" -f $_.TargetObject.Line, $_.TargetObject.ErrorText) } Add-Type -TypeDefinition $failedCode -Language CSharp -ErrorAction Stop; [Failed]::Main() }"