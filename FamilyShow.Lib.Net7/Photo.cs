using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FamilyShow.Lib.Net7;

/// <summary>
/// Simple representation of a serializable photo associated with the Person class
/// </summary>
[Serializable]
public class Photo : INotifyPropertyChanged
{
    #region Fields and Constants

    // The constants specific to this class
    public static class Const
    {
        public const string PhotosFolderName = "Images";
    }

    private string relativePath;
    private bool isAvatar;

    #endregion

    #region Properties

    /// <summary>
    /// The relative path to the photo.
    /// </summary>
    public string RelativePath
    {
        get => relativePath;
        set
        {
            if (relativePath != value)
            {
                relativePath = value;
                OnPropertyChanged(nameof(relativePath));
            }
        }
    }

    /// <summary>
    /// The fully qualified path to the photo.
    /// </summary>
    public string FullyQualifiedPath
    {
        get
        {
            var tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ApplicationFolderName);
            tempFolder = Path.Combine(tempFolder, App.AppDataFolderName);
            return Path.Combine(tempFolder, relativePath);
        }
        set
        {
            // This empty setter is needed for serialization.
        }
    }

    /// <summary>
    /// Whether the photo is the avatar photo or not.
    /// </summary>
    public bool IsAvatar
    {
        get => isAvatar;
        set
        {
            if (isAvatar != value)
            {
                isAvatar = value;
                OnPropertyChanged(nameof(IsAvatar));
            }
        }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Empty constructor is needed for serialization
    /// </summary>
    public Photo()
    {
    }

    /// <summary>
    /// Constructor for Photo. Copies the photoPath to the images folder
    /// </summary>
    public Photo(string photoPath)
    {
        if (!string.IsNullOrEmpty(photoPath))
            // Copy the photo to the images folder
            relativePath = Copy(photoPath);
    }

    #endregion

    #region Methods

    public override string ToString()
    {
        return FullyQualifiedPath;
    }

    /// <summary>
    /// Copies the photo file to the application photos folder. 
    /// Returns the relative path to the copied photo.
    /// </summary>
    private static string Copy(string fileName)
    {
        // The photo file being copied
        FileInfo fileInfo = new(fileName);

        // Absolute path to the application folder
        var appLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), App.ApplicationFolderName);
        appLocation = Path.Combine(appLocation, App.AppDataFolderName);

        // Absolute path to the photos folder
        var photoLocation = Path.Combine(appLocation, Const.PhotosFolderName);

        // Fully qualified path to the new photo file
        var photoFullPath = Path.Combine(photoLocation, fileInfo.Name);

        // Relative path to the new photo file
        var photoRelLocation = Path.Combine(Const.PhotosFolderName, fileInfo.Name);

        // Create the appLocation directory if it doesn't exist
        if (!Directory.Exists(appLocation)) Directory.CreateDirectory(appLocation);

        // Create the photos directory if it doesn't exist
        if (!Directory.Exists(photoLocation)) Directory.CreateDirectory(photoLocation);

        // Copy the photo.
        try
        {
            fileInfo.CopyTo(photoFullPath, true);
        }
        catch
        {
            // Could not copy the photo. Handle all exceptions 
            // the same, ignore and continue.
        }

        return photoRelLocation;
    }

    public void Delete()
    {
        try
        {
            File.Delete(FullyQualifiedPath);
        }
        catch
        {
            // Could not delete the file. Handle all exceptions
            // the same, ignore and continue.
        }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}

/// <summary>
/// Collection for photos.
/// </summary>
[Serializable]
public class PhotoCollection : ObservableCollection<Photo>
{
}