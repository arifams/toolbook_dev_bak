-- User profile should have only english language.

Update languages set IsActive = 0
Update languages set IsActive = 1 where LanguageCode = 'EN'