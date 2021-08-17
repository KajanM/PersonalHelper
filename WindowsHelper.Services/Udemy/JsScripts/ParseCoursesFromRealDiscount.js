function parseCourses() {
  const courseElements = Array.from(document.querySelectorAll('.row.main-bg .col-sm-12.col-md-6.col-lg-4.col-xl-4'))

  return courseElements.map(courseEle => {
    const Title = courseEle.querySelector('h3').textContent
    const Link = courseEle.querySelector('a').href
    const ShortDescription = courseEle.querySelector('.card-text').textContent
    const Category = courseEle.querySelector('.card-cat').textContent
    const CouponPublishedTime = courseEle.querySelector('.card-date span').textContent
    const ExpiresIn = courseEle.querySelector('.card-duration div:nth-of-type(2)').textContent.trim()
    const IsEditorsChoice = !!courseEle.querySelector('img.card-badge')

    return {
      Title,
      Link,
      ShortDescription,
      Category,
      CouponPublishedTime,
      ExpiresIn,
      IsEditorsChoice,
    }
  })
}
    