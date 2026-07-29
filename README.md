

## Task 2: Magic words

"position" is interpreted as the position of the avatar in the dialogue.

Edge case handling:
- Simplified edge case handling
-- avatars with the same name: keep the first one from the list
-- avatars not appearing in any dialogue: discard entirely since no image will be shown
-- character dialogues with no avatar or with avatar fetch error/timeout: show a default icon for the avatar

- A little more complex edge case handling:
-- Ensure that there is always at least one character on the left and one on the right in the dialogue. In this specific case, sheldon appears in the avatar list in both left and right positions and since it's the only character with a left attribute, left would be chosen for his position.
-- If there is a character with several avatar entries, try fetching all of the available avatars and choose randomly among the ones properly fetched


Emojis downloaded from https://github.com/jdecked/twemoji