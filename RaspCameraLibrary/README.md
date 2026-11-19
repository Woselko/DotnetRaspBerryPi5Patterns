# RaspCameraLibrary

C# wrapper for Raspberry Pi camera tools (`rpicam-hello`, `rpicam-vid`, `rpicam-still`) targeting Raspberry Pi 5.

Version **1.1.0** adds high-level capture APIs, diagnostics, process safety, profiles, MJPEG helpers, multi-camera capture, output validation, DI (`ICameraService`), metadata helpers, and `libcamera-*` fallback.

## Install (GitHub Packages)

```bash
dotnet nuget add source https://nuget.pkg.github.com/Woselko/index.json \
  --name github-woselko \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_PAT \
  --store-password-in-clear-text

dotnet add package RaspCameraLibrary --version 1.1.0
```

## Quick start

```csharp
using RaspCameraLibrary.MultiCamera;
using RaspCameraLibrary.Profiles;
using RaspCameraLibrary.Services;
using RaspCameraLibrary.Streaming;

// One-liners
await CameraProfiles.QuickPreviewAsync(cameraId: 0, timeoutMs: 5000);
await CameraProfiles.RecordH264Async("video0.h264", cameraId: 0, durationMs: 5000);
await CameraProfiles.SnapshotJpgAsync("shot.jpg", cameraId: 0);

// Both cameras
await MultiCameraCapture.RecordBothH264Async("video0.h264", "video1.h264", durationMs: 5000);

// MJPEG frames
await using var mjpeg = new MjpegFrameSource();
await mjpeg.StartAsync(cameraId: 0);
await foreach (var frame in mjpeg.ReadFramesAsync()) { /* JPEG bytes */ }

// DI
services.AddRaspCamera();
// ICameraService → CaptureVideoAsync / CaptureStillAsync / PreviewAsync / GetCamerasAsync
```

## Features

| Area | API |
|------|-----|
| High-level capture | `ICameraService` / `CameraService` |
| Camera discovery | `CameraSelector.GetCamerasAsync()` |
| Health check | `CameraDiagnostics.CheckAsync()` |
| Safe processes | `CameraProcessSession` (`IAsyncDisposable`) |
| Profiles | `CameraProfiles.QuickPreviewAsync` / `RecordH264Async` / `SnapshotJpgAsync` |
| MJPEG stream | `MjpegFrameSource`, `MjpegHttpWriter` |
| Multi-camera | `MultiCameraCapture.RecordBothH264Async` |
| Output targets | `CaptureOutput.File` / `Stdout` / `NamedPipe` |
| Metadata | `CaptureMetadata.Apply(...)` |
| Binary fallback | `CameraBinaryResolver` (`rpicam-*` → `libcamera-*`) |

## Requirements

- Raspberry Pi OS with `rpicam-apps` (or legacy `libcamera-apps`)
- Camera module enabled
