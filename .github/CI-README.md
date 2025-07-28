# CI/CD Pipeline Documentation

This repository includes a comprehensive GitHub Actions CI/CD pipeline that automatically builds the project and optionally publishes NuGet packages.

## Pipeline Overview

The CI pipeline is defined in `.github/workflows/ci.yml` and provides the following functionality:

### Automatic Build
- **Triggers**: Push to `main` or `develop` branches, pull requests
- **Actions**: Restores dependencies, builds the project, runs tests
- **Caching**: NuGet packages are cached to improve build performance

### Automatic NuGet Publishing
- **Triggers**: 
  - GitHub releases (for stable versions)
  - Push to `main` branch (for pre-release versions)
- **Versioning**: 
  - Release versions use the git tag (e.g., `v1.0.0` → `1.0.0`)
  - Main branch builds use alpha versioning (e.g., `1.0.0-alpha.123`)
  - Other branches use dev versioning (e.g., `1.0.0-dev.123`)

## Setup Requirements

### 1. NuGet API Key
To enable NuGet publishing, you need to configure a secret in your GitHub repository:

1. Go to your repository on GitHub
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Your NuGet.org API key

### 2. Create a NuGet.org API Key
1. Sign in to [nuget.org](https://nuget.org)
2. Go to **Account Settings** → **API Keys**
3. Click **Create**
4. Choose appropriate permissions (Push new packages and package versions)
5. Select package pattern or specific packages
6. Copy the generated API key

## Usage

### Development Builds
- Every push to `main` or `develop` branches triggers a build
- Pull requests also trigger builds to validate changes
- No packages are published for development builds (except main branch alpha versions)

### Release Process
1. **Create a Release**: 
   - Go to your GitHub repository
   - Click **Releases** → **Create a new release**
   - Choose or create a tag (e.g., `v1.0.0`)
   - Add release notes
   - Click **Publish release**

2. **Automatic Publishing**:
   - The CI pipeline automatically triggers on release creation
   - Builds the project with the release version
   - Creates NuGet packages
   - Publishes to NuGet.org (if API key is configured)

### Version Examples
- Tag `v1.0.0` → NuGet version `1.0.0`
- Tag `v1.2.3-beta` → NuGet version `1.2.3-beta`
- Main branch build #45 → NuGet version `1.0.0-alpha.45`
- Feature branch build #67 → NuGet version `1.0.0-dev.67`

## Package Information
The generated NuGet package includes:
- **Package ID**: `Hermes.Core`
- **Title**: Hermes Core
- **Description**: Core abstractions for event-driven architecture and messaging in .NET applications
- **License**: MIT
- **Tags**: messaging, events, commands, cqrs, event-driven, architecture, abstractions
- **Source linking**: Enabled for debugging support
- **Symbol packages**: Generated for debugging

## Troubleshooting

### Build Failures
- Check the **Actions** tab in your GitHub repository
- Review build logs for specific error messages
- Ensure all dependencies are properly restored

### Publishing Failures
- Verify the `NUGET_API_KEY` secret is configured correctly
- Check that the API key has appropriate permissions
- Ensure package version doesn't already exist on NuGet.org

### Version Conflicts
- NuGet.org doesn't allow overwriting existing versions
- Use pre-release versions for testing (e.g., `1.0.0-beta1`)
- Increment version numbers for new releases