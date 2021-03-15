
1. Download The Application Programming Interface --> [Click Me](Add Link)

2. Add Nihon's Application Programming Interface As A Reference In Your Project

3. Making A Variable And Adding Usage

```csharp
using MercuryLibrary;

MercuryLibrary.Functions MercuryLib = new MercuryLibrary.Functions();
```

4. Using Functions

```csharp
private void InjectButton(object sender EventArgs e)
{
    MercuryLib.Inject(LuaPipe DllName); //Enter LuaPipe And Dll Name
}

private void ExecuteButton(object sender EventArgs e)
{
    MercuryLib.ExecuteScript(LuaPipe, DllName, Avalon.Text); //In This Instance We Are Using Avalon
}
```
