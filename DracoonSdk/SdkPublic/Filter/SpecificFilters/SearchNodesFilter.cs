﻿namespace Dracoon.Sdk.Filter {
    /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/SearchNodesFilter/*'/>
    public class SearchNodesFilter : DracoonFilter {
        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/Type/*'/>
        public static NodeTypeFilter Type {
            get {
                return new NodeTypeFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/IsFavorite/*'/>
        public static IsFavoriteFilter IsFavorite {
            get {
                return new IsFavoriteFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/ParentPath/*'/>
        public static ParentPathFilter ParentPath {
            get {
                return new ParentPathFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/UpdatedBy/*'/>
        public static UpdatedByFilter UpdatedBy {
            get {
                return new UpdatedByFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/CreatedBy/*'/>
        public static CreatedByFilter CreatedBy {
            get {
                return new CreatedByFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/FileType/*'/>
        public static FileTypeFilter FileType {
            get {
                return new FileTypeFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/Classification/*'/>
        public static ClassificationFilter Classification {
            get {
                return new ClassificationFilter();
            }
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddNodeTypeFilter/*'/>
        public void AddNodeTypeFilter(DracoonFilterType<NodeTypeFilter> typeFilter) {
            CheckFilter(typeFilter, nameof(typeFilter));
            FiltersList.Add(typeFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddIsFavoriteFilter/*'/>
        public void AddIsFavoriteFilter(DracoonFilterType<IsFavoriteFilter> isFavoriteFilter) {
            CheckFilter(isFavoriteFilter, nameof(isFavoriteFilter));
            FiltersList.Add(isFavoriteFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddParentPathFilter/*'/>
        public void AddParentPathFilter(DracoonFilterType<ParentPathFilter> parentPathFilter) {
            CheckFilter(parentPathFilter, nameof(parentPathFilter));
            FiltersList.Add(parentPathFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddUpdatedByFilter/*'/>
        public void AddUpdatedByFilter(DracoonFilterType<UpdatedByFilter> updatedByFilter) {
            CheckFilter(updatedByFilter, nameof(updatedByFilter));
            FiltersList.Add(updatedByFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddFileTypeFilter/*'/>
        public void AddFileTypeFilter(DracoonFilterType<FileTypeFilter> fileTypeFilter) {
            CheckFilter(fileTypeFilter, nameof(fileTypeFilter));
            FiltersList.Add(fileTypeFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddClassificationFilter/*'/>
        public void AddClassificationFilter(DracoonFilterType<ClassificationFilter> classificationFilter) {
            CheckFilter(classificationFilter, nameof(classificationFilter));
            FiltersList.Add(classificationFilter);
        }

        /// <include file="SpecificFilterDoc.xml" path='docs/members[@name="searchNodesFilter"]/AddCreatedByFilter/*'/>
        public void AddCreatedByFilter(DracoonFilterType<CreatedByFilter> createdByFilter) {
            CheckFilter(createdByFilter, nameof(createdByFilter));
            FiltersList.Add(createdByFilter);
        }
    }
}