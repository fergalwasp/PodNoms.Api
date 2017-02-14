SELECT Id, Title, Slug, AudioFileSize, Processed, ProcessingStatus FROM PodcastEntries
DELETE FROM PodcastEntries WHERE Processed  = 0