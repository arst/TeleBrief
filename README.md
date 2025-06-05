# TeleBrief

TeleBrief is a CLI tool that automatically summarizes content from multiple Telegram channels into a concise, well-organized brief. It uses AI to process messages, remove noise, and present only meaningful facts grouped by categories.

## Features

- 🤖 AI-powered summarization using Semantic Kernel
- 📱 Fetches messages from configured Telegram channels
- 🗂️ Groups related information into 3-6 clear categories
- 🚫 Filters out noise, speculation, ads, and irrelevant content
- 📊 Presents information in a clean, structured format

## Prerequisites

- .NET 9.0 or higher
- Configuration files with required settings

## Configuration

The application requires two configuration files:

1. `app.settings.json` (required) - Main application settings
2. `local.settings.json` (optional) - Local environment overrides

Example configuration:
```json
{
  "TelegramChannels": [
    "channel1",
    "channel2"
  ],
  "BatchSize": 10
}
```

## Usage

To get today's news summary:
```bash
dotnet run -- news
```

## Output Format

The tool generates summaries in a structured format:

```
Category Name:
- Fact 1
- Fact 2
- Fact 3

Another Category:
- Fact A
- Fact B
```

## License

This project is open source and available under the MIT license.
