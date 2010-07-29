﻿using Orchard.ContentManagement.FieldStorage.InfosetStorage;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Tasks.Indexing;
using System.Collections.Generic;

namespace Orchard.Indexing.Handlers {
    /// <summary>
    /// Intercepts the ContentHandler events to create indexing tasks when a content item 
    /// is published, and to delete them when the content item is unpublished.
    /// </summary>
    public class CreateIndexingTaskHandler : ContentHandler {
        private const string SearchIndexName = "Search";
        private readonly IIndexingTaskManager _indexingTaskManager;
        private readonly IEnumerable<IIndexNotifierHandler> _indexNotifierHandlers;

        public CreateIndexingTaskHandler(
            IIndexingTaskManager indexingTaskManager,
            IEnumerable<IIndexNotifierHandler> indexNotifierHandlers
            ) {
            _indexingTaskManager = indexingTaskManager;
            _indexNotifierHandlers = indexNotifierHandlers;

            OnPublishing<ContentPart>(CreateIndexingTask);
            OnRemoved<ContentPart>(RemoveIndexingTask);
        }

        void CreateIndexingTask(PublishContentContext context, ContentPart part) {
            _indexingTaskManager.CreateUpdateIndexTask(context.ContentItem);
            UpdateIndex();
        }

        void RemoveIndexingTask(RemoveContentContext context, ContentPart part) {
            _indexingTaskManager.CreateDeleteIndexTask(context.ContentItem);
            UpdateIndex();
        }

        private void UpdateIndex() {
            foreach (var handler in _indexNotifierHandlers) {
                handler.UpdateIndex(SearchIndexName);
            }
        }
    }
}
