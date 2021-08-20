function parseLibgenSearchResults() {
  const links = []
  const results = Array.from(document.querySelectorAll('.c tr'))

  for (let i = 1; i < results.length; i++) {
    const book = results[i]

    const columns = book.querySelectorAll('td')
    const id = columns[0].textContent
    const authors = Array.from(columns[1].querySelectorAll('a')).map(linkEle => linkEle.textContent)
    const title = columns[2].querySelector('a').childNodes[0].textContent
    const year = columns[4].textContent
    const pageCount = columns[5].textContent
    const language = columns[6].textContent
    const size = columns[7].textContent
    const extension = columns[8].textContent
    const mirrors = [];
    for (let j = 9; j <= 13; j++) {
      mirrors.push(columns[j]?.childNodes[0]?.href)
    }

    const bookData = {
      id,
      authors,
      title,
      size,
      year,
      pageCount,
      language,
      extension,
      mirrors
    }
    links.push(bookData)
  }
  
  return links;
}