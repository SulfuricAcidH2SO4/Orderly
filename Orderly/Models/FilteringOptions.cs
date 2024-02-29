using Orderly.Database.Modules;

namespace Orderly.Models
{
    public class FilteringOptions : BindableBase
    {
        private bool favoriteOnTop = true;
        private SortingOption sortingOption = SortingOption.AlphabeticalAscending;

        public bool FavoriteOnTop
        {
            get => favoriteOnTop;
            set
            {
                SetProperty(ref favoriteOnTop, value);
            }
        }
        public SortingOption SortingOption
        {
            get => sortingOption;
            set
            {
                SetProperty(ref sortingOption, value);
            }
        }

    }

    public enum SortingOption
    {
        AlphabeticalDescending,
        AlphabeticalAscending,
        OldestAdded,
        NewestAdded,
        OldestEdited,
        NewestEdited
    }
}
