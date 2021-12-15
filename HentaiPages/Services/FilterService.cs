using HentaiPages.Models;

namespace HentaiPages.Services
{
    public interface IFilterService
    {
        void Reset();
        GalleryFilter GetCurrentFilters();
        void SetData(GalleryFilter filters);
    }

    public class FilterService : IFilterService
    {
        private GalleryFilter _galleryFilter;

        public FilterService()
        {
            _galleryFilter = new GalleryFilter();
        }

        public void SetData(GalleryFilter filters)
        {
            _galleryFilter = filters;
        }

        public void Reset()
        {
            _galleryFilter = new GalleryFilter();
        }

        public GalleryFilter GetCurrentFilters() => _galleryFilter;
    }
}