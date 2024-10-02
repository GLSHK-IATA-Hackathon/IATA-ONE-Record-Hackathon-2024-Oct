export function push(subapp) {
  history.pushState(null, subapp, subapp);
}

export function getLanguageFromUrl() {
  return location.pathname
    .split("/")
    .filter((val) => !!val)
    .at(0);
}
