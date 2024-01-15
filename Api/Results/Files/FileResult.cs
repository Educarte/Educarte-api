namespace Api.Results.Files;

/// <summary>
/// File Result
/// </summary>
public class FileResult
{
    /// <summary>
    /// Items
    /// </summary>
    public IEnumerable<Item> Items { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Name of item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }
    }
}
