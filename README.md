# 🎥 CaptureTool

**CaptureTool** is a windows (cross-platform planned) desktop application built with Avalonia UI that records screen and audio, transcribes speech, and outputs a final video with optional subtitles — inspired by the functionality of [Scribe](https://scribehow.com/).

This project is currently in progress as a technical showcase for an interview. While the full capture functionality is under development, the foundation has been designed to reflect best practices in architecture, modularity, and UI/UX parity with Scribe.

---

## ✨ Features (Planned + In Progress)

| Feature                        | Status      |
|-------------------------------|-------------|
| Modern styled UI (Scribe-like) | ✅ Done      |
| Custom-themed buttons & toggles | ✅ Done      |
| Application settings (DI + JSON persistence) | ✅ Done      |
| Menu system w/ nested options | ✅ Done      |
| Screen recording service       | 🔄 Interface defined |
| Audio recording service        | 🔄 Interface defined |
| Transcription service (Whisper integration) | 🔲 Planned    |
| Muxing service (video + audio + optional subtitles) | 🔲 Planned    |
| Session-based folder output (timestamped) | 🔄 In progress |
| Test and refactor for cross-platform functionality | 🔲 Planned    |
| Audio device and screen selection | 🔲 Planned    |
| Logging service ( Serilog or NLog ) | 🔲 Planned    |

---

## 🧩 Architecture Overview

CaptureTool is designed around clean separation of concerns and SOLID principles. The app is composed of the following services:

### `ICaptureService`
High-level orchestrator that coordinates audio, screen, transcription, and muxing workflows.

### `IScreenRecordService`
Responsible for screen capture using configurable inputs (e.g., screen index, resolution, framerate | currently uses main desktop).

### `IAudioRecordService`
Handles audio capture from a selected input device. (currently uses default)

### `ITranscriptionService`
Processes recorded audio through Whisper or other engines to generate `.srt` and `.txt` transcripts.

### `IMuxingService`
Combines audio, video, and optionally hardcoded subtitles into a final `.mp4` file.

### `ISettingsService`
Manages persistent application settings using a local JSON file:
```
C:\ProgramData\CaptureTool\appsettings.json
```

---

## 💡 Design Goals

- ✅ **Cross-platform (Windows/macOS)** via Avalonia UI
- ✅ **Dependency Injection** via `Microsoft.Extensions.DependencyInjection`
- ✅ **MVVM Architecture** with `CommunityToolkit.Mvvm`
- ✅ **Modular & extensible** service-based design
- ✅ **Scalable for future features** like subtitle overlay, cloud transcription, and export presets

---

## 📂 Example Directory Layout

Each recording creates a new folder under the user’s capture directory:

```
~/Videos/CaptureTool/Captures/2024-04-21_12-32-15/
├── screen.mp4
├── audio.wav
├── transcript.srt
├── transcript.txt
└── final_output.mp4
```

---

## 🛠️ Tech Stack

- **UI:** [Avalonia UI](https://avaloniaui.net/)
- **MVVM:** CommunityToolkit.Mvvm
- **DI:** Microsoft.Extensions.DependencyInjection
- **Serialization:** System.Text.Json
- **Recording (planned):** FFmpeg

---

## 🚧 Current Status

> The UI and application structure are fully implemented. Service interfaces are defined. Integration of FFmpeg and Whisper for screen/audio recording and transcription is next.

You can check progress and services inside the [`Services`](./source/CaptureTool/Services) directory.

---

## 🗺️ Roadmap

- [ ] Implement `ScreenRecordService` using FFmpeg
- [ ] Implement `AudioRecordService` using FFmpeg
- [ ] Add basic `TranscriptionService` using Whisper
- [ ] Add muxing pipeline to stitch audio + video + subtitles
- [ ] Add start/stop recording logic via `CaptureService`
- [ ] Implement audio device selection
- [ ] Implement screen or app selection
- [ ] Implement logging

---

## 📸 Screenshots
<p float="left">
<img src="screenshots/MainWindow.png" alt="Main UI" width="300"/>

<img src="screenshots/ButtonPressAndHold.png" alt="Button Press & Hold" width="300"/>

<img src="screenshots/Menu.png" alt="Dropdown menu & CheckBox" width="400"/>
</p>

---

## 🧠 Why I Built This

This project was built as a reverse-engineered, developer-focused replica of the Scribe desktop app. I wanted to showcase:

- My UI skills (restyling controls, layouts, I learned a lot of the styling along the way)
- My ability to architect scalable cross-platform applications
- My drive to learn on the fly, even when I initially don’t know how to do something
- My initiative to replicate a production-grade tool with zero access to its source

---

⭐ If you're reviewing this for a technical interview — thank you!
