SELECT Id, Title, Slug, AudioFileSize, Processed, ProcessingStatus FROM PodcastEntries
DELETE FROM PodcastEntries WHERE Processed  = 0
SELECT Id, Title, Slug, AudioFileSize, Processed, ProcessingStatus FROM PodcastEntries WHERE Processed  = 0
DELETE FROM PodcastEntries WHERE Id >= 57
SELECT * FROM Podcasts
SELECT * FROM PodcastEntries
DELETE FROM Podcasts
DELETE FROM PodcastEntries



