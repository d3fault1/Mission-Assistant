# Flight Planner

WPF planner for drawing flight routes on a map, with aircraft performance data stored in a SQLite database.

---

## Overview

- Draw lines, polygons, circles, routes, diversion legs, checkpoints, and images on a map.
- Open and save the drawing as a `.pma` file.
- Keep climb, descent, fuel, and speed data per aircraft.
- Show or hide a chart image over the map.
- Export the drawing to `MapDrawing.png`.

---

## Tech Stack

| Component | Technology |
| --- | --- |
| **Language** | C# |
| **UI** | WPF, .NET Framework 4.7.2 |
| **Map** | GMap.NET |
| **Data** | SQLite |

---

## Project Structure

```
Mission-Assistant/
├── Mission Assistant.sln
├── MainWindow.xaml
├── Route/
├── Polygon/
├── Circle/
├── Checkpoint/
├── MapLine/
├── DataManagement/
├── AircraftDataChart.xaml
├── Platforms.xaml
└── SaveLoad/
```

---

## Prerequisites

- Windows
- Visual Studio with the .NET desktop workload (.NET Framework 4.7.2)
- A network connection for map tiles
- `BAFMap.png`

---

## Setup

Open `Mission Assistant.sln` and restore the NuGet packages.

Copy `BAFMap.png` into the output folder (`bin\Debug` or `bin\Release`), then start the project.

---

## Usage

| Control | Result |
| --- | --- |
| **Open** | Loads a `.pma` file and replaces the current drawing |
| **Save** | Writes a `.pma` file |
| **Map** | Shows or hides the base map |
| **Draw** | Draws lines, polygons, and circles |
| **Route** | Adds waypoints for a selected aircraft |
| **Diversion, checkpoint** | Adds a diversion leg or a checkpoint on a route |
| **Image** | Places a picture on the map |
| **Platforms** | Opens the aircraft list and the performance chart |
| **Export** | Writes `MapDrawing.png` |
