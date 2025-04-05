mergeInto(LibraryManager.library, {
  UploadScoreToFirebase: function(playerIdPtr, stage, score) {
    const playerId = UTF8ToString(playerIdPtr);
    const timestamp = new Date().toISOString();

    firebase.database().ref("scores").push({
      playerId,
      stage,
      score,
      timestamp
    });
  }
});
