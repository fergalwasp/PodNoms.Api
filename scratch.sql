SELECT Id, Title, Slug, AudioFileSize, Processed, ProcessingStatus FROM PodcastEntries
DELETE FROM PodcastEntries WHERE Processed  = 0
SELECT Id, Title, Slug, AudioFileSize, Processed, ProcessingStatus FROM PodcastEntries WHERE Processed  = 0
DELETE FROM PodcastEntries WHERE Id >= 57
