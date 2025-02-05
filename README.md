# SolutionToText

This C# console application recursively traverses a directory, collects files with specified extensions (while respecting .gitignore patterns), and combines their contents into a single text file.

## 🛠️ Configuration

The application uses an appsettings.json file for configuration, which is divided into two main sections:

### Settings

The **Settings** section allows you to select the active configuration for analyzing the project.

#### Parameters

- `SelectedConfigurationTitle` - Specifies which configuration from the Configurations section will be used by default during the application's execution. You can change the active configuration by setting this parameter to the title of any predefined configuration from the list.

#### Example

```json
{
  "Settings": {
    "SelectedConfigurationTitle": "C# with website"
  }
}
```

### Configurations

The **Configurations** section contains a set of predefined configurations that customize the project analysis parameters.

#### Parameters

- `Title` - A unique name that identifies the configuration.
- `IncludeFileExtensions` - An array of file extensions to include in the analysis; only files with these extensions will be processed.
- `ExcludePatterns`: An array of file or directory titles to exclude from the analysis; files or directories matching these patterns will be ignored.

#### Example

```json
{
  "Configurations": [
    {
      "Title": "C#",
      "IncludeFileExtensions": [".cs"],
      "ExcludePatterns": ["obj", ".git", "bin", ".idea", ".vs"]
    },
    {
      "Title": "C# with website",
      "IncludeFileExtensions": [".cs", ".js", ".css", ".cshtml", ".cshtml.cs"],
      "ExcludePatterns": ["obj", ".git", "bin", ".idea", ".vs", "lib"]
    }
  ]
}
```

## 🏗️ Project Structure

```
SolutionToText/
├── Interfaces/          # Interface definitions
├── Models/              # Data models and configurations
├── Services/            # Core business logic implementation
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration file
```

## 💻 Usage

1. Download the desired version from the releases section.
2. Unzip the downloaded file.
3. Run the application 
4. Enter the path to your solution folder when prompted 
5. The application will:
   - Traverse the directory structure
   - Apply `.gitignore` patterns
   - Collect files matching configured extensions
   - Generate a `result.txt` file on your desktop
   - Automatically open the generated file

## 🔍 Output Format

The generated `result.txt` file contains:
1. A directory tree structure showing all files and folders
2. Contents of each included file, preceded by its relative path

Example:
```
- SolutionToText
-- Program.cs
-- Services
--- FileProcessor.cs

File content \SolutionToText\Program.cs:
[file contents here]

File content \SolutionToText\Services\FileProcessor.cs:
[file contents here]
```

## 📝 License

This project is licensed under the [MIT License](LICENSE).
