mergeInto(LibraryManager.library, {
  UploadSessionToFirebase: function (jsonPtr) {
    const json = UTF8ToString(jsonPtr);
    window.UploadSessionToFirebase(json);
  },
  UploadPlayLogToFirebase: function (jsonPtr) {
    const json = UTF8ToString(jsonPtr);
    window.UploadPlayLogToFirebase(json);
  }
});
