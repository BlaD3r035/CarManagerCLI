# CarManagerCLI 

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-Programming-green?logo=csharp) ![Platform](https://img.shields.io/badge/Platform-CLI-lightgrey?logo=windows-terminal)


A simple (CLI) c# project to manage car dealers, their vehicles, and user sessions.  


---

## Features
- Manage multiple **car dealers**.
- Add, edit, and list **cars** with details:
  - Plate, VIN, Brand, Model, Year, Color.
- Track **cars currently in use** and their history of users.
- Store **session data** (active dealer, presence status).
- Persist data using **JSON configuration files** (`Dealers.json` and `Session.json`).

---

## Project Structure

CarManagerCLI/
> │── Client/ # CLI logic (only Vehicle manager)

> │── Data/ # json files

> │── Config/ # Configuration management (Json data op)

> │── Controllers/ # Dealers controller

> │── Models/ # Core models (Car, CarDealer)

> │── Program.cs # Entry point and main menu

> │── CarManagerCLI.csproj

# Installation & Usage
## Via zip:
https://github.com/BlaD3r035/CarManagerCLI/releases/tag/Release
Download, unzip, enjoy💕
## Via repo 
### clone repo
```bash
git clone https://github.com/BlaD3r035/CarManagerCLI
cd CarManagerCLI
```
### Build and run project
```bash 
dotnet build
dotnet run
```

## Extra:
This is basic C# code for practice. You can use it as a reference if you want to experiment with similar examples.
