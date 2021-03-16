
1. Download The Class Library --> [Click Me](https://github.com/Nihon-Development/Mercury-Library/blob/main/Mercury%20Dll/Mercury%20Library.dll?raw=true)

2. Add Mercury's Library As A Reference In Your Project

3. Making A Variable And Adding Usage

```csharp
using MercuryLibrary;

MercuryLibrary.Functions MercuryLib = new MercuryLibrary.Functions();
```

4. Set Mercury Settings

```csharp
MercuryLib.Settings.LuaPipe = ""; //Put These Under A Loaded Event
MercuryLib.Settings.DllName = "";
```

5. Using Functions

```csharp
private void InjectButton(object sender EventArgs e)
{
    MercuryLib.Inject(); 
}

private void ExecuteButton(object sender EventArgs e)
{
    MercuryLib.ExecuteScript(Avalon.Text); //In This Instance We Are Using Avalon
    MercuryLib.ExecuteScript("print("Made With Mercury Library")"); // Instance Of Executing A Script Directly
}
```

## Pipe List

```
EasyExploits --> ocybedam

WeAreDevs Lua --> WeAreDevsPublicAPI_Lua
WeAreDevs LuaC --> WeAreDevsPublicAPI_LuaC
WeAreDevs CMD --> WeAreDevsPublicAPI_CMD

ShadowCheats --> qa6uPWYHm1LeUFsLosFF

CheatSquad --> taStqdau1Ch1ee33

Oxygen U --> OxygenU

Acrylix --> Acrylix

Sk8r/CheatSquad --> zxc

Krnl --> krnlpipe

RobloxHacksAPI --> RobloxHacks
```
