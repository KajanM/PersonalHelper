function parseCourseDetailsFromUdemy() {
  // const title = document.querySelector('.clp-lead__title').textContent.trim()
  // const shortDescription = document.querySelector('.clp-lead__title').textContent.trim()
  const discountPercentage = document.querySelector('.udlite-clp-percent-discount span:nth-of-type(2)').textContent
  const priceAfterCouponIsApplied = document.querySelector('.udlite-clp-discount-price span:nth-of-type(2)').textContent
  const courseDuration = document.querySelector('[data-purpose="video-content-length"]').textContent
  const rating = document.querySelector('[data-purpose="rating"]').textContent
  const enrolledStudentsCount = document.querySelector('[data-purpose="enrollment"]').textContent.trim()
  const lastUpdated = document.querySelector('.last-update-date span:nth-of-type(2)').textContent.trim()
  const courseProviderRating = document.querySelector('[data-purpose="instructor-bio"] ul li .udlite-block-list-item-content').textContent
  const data = JSON.parse(document.querySelector('#schema_markup script').innerHTML)[0]
  const category = document.head.querySelector('[property="udemy_com:category"]').content
  
  return {
    title: data.name,
    shortDescription: data.description,
    language: data.inLanguage,
    courseUri: data['@id'],
    imageUri: data.image, 
    discountPercentage,
    priceAfterCouponIsApplied,
    courseDuration,
    rating,
    enrolledStudentsCount,
    lastUpdated,
    courseProviderName: data.provider.name,
    courseProviderRating,
    courseProviderUri: data.provider.sameAs,
    category,
    ratingCount: data.aggregateRating?.ratingCount || 0,
    ratingValue: parseFloat(data.aggregateRating?.ratingValue || "0")
  }
}