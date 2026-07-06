/* =========================================================================
   MayXayDung Marketplace — shared behaviour + i18n (EN / VI)
   Usage: add data-i18n="key" to any element; text swaps on toggle.
   For placeholders use data-i18n-ph="key".
   ========================================================================= */

const I18N = {
  vi: {
    // top bar + nav
    "top.support": "Hỗ trợ: 1900 6060",
    "top.help": "Trợ giúp",
    "nav.buy": "Mua bán",
    "nav.categories": "Danh mục",
    "nav.dealers": "Nhà cung cấp",
    "nav.pricing": "Gói quảng cáo",
    "nav.login": "Đăng nhập",
    "nav.post": "Đăng tin",
    "nav.dashboard": "Quản lý tin",
    "search.what": "Bạn cần tìm máy gì?",
    "search.what.label": "Từ khoá",
    "search.cat.label": "Danh mục",
    "search.loc.label": "Khu vực",
    "search.cat.all": "Tất cả danh mục",
    "search.loc.all": "Toàn quốc",
    "search.btn": "Tìm kiếm",
    "search.popular": "Tìm nhiều:",

    // hero
    "hero.eyebrow": "Sàn giao dịch máy xây dựng #1 Việt Nam",
    "hero.title": "Mua bán máy móc & thiết bị xây dựng",
    "hero.sub": "Hàng chục nghìn tin đăng từ nhà cung cấp uy tín. Tìm đúng thiết bị, liên hệ trực tiếp người bán — không qua trung gian.",
    "hero.stat1": "Tin đang đăng",
    "hero.stat2": "Nhà cung cấp",
    "hero.stat3": "Giao dịch/tháng",

    // sections
    "cats.title": "Danh mục thiết bị",
    "cats.sub": "Chọn nhóm máy bạn quan tâm",
    "feat.title": "Tin nổi bật",
    "feat.sub": "Tin được ưu tiên hiển thị",
    "feat.viewall": "Xem tất cả",
    "recent.title": "Tin mới đăng",
    "trust.title": "Vì sao chọn chúng tôi",
    "trust.1.t": "Người bán được xác minh",
    "trust.1.d": "Nhà cung cấp xác thực giấy phép kinh doanh trước khi đăng tin.",
    "trust.2.t": "Liên hệ trực tiếp",
    "trust.2.d": "Gọi hoặc nhắn tin thẳng cho người bán, không mất phí trung gian.",
    "trust.3.t": "Kiểm duyệt tin đăng",
    "trust.3.d": "Đội ngũ quản trị rà soát và gỡ tin vi phạm mỗi ngày.",
    "seller.eyebrow": "Dành cho người bán",
    "seller.title": "Đăng bán thiết bị của bạn tiếp cận hàng nghìn người mua",
    "seller.sub": "Tạo tin miễn phí trong 2 phút. Đẩy tin lên đầu kết quả tìm kiếm với gói quảng cáo.",
    "seller.cta": "Đăng tin miễn phí",
    "seller.cta2": "Xem gói quảng cáo",

    // listing labels
    "lbl.boosted": "Ưu tiên",
    "lbl.verified": "Đã xác minh",
    "lbl.new": "Mới",
    "lbl.used": "Đã qua sử dụng",
    "lbl.contact": "Liên hệ người bán",
    "lbl.call": "Gọi ngay",
    "lbl.message": "Nhắn tin",
    "lbl.negotiable": "Thương lượng",

    // footer
    "foot.about": "Sàn giao dịch mua bán và cho thuê máy móc, thiết bị xây dựng hàng đầu Việt Nam.",
    "foot.explore": "Khám phá",
    "foot.forsellers": "Người bán",
    "foot.support": "Hỗ trợ",
    "foot.rights": "© 2026 MayXayDung. Bản quyền đã được bảo hộ.",

    // search page
    "sp.results": "kết quả cho",
    "sp.filters": "Bộ lọc",
    "sp.sort": "Sắp xếp",
    "sp.sort.relevant": "Liên quan nhất",
    "sp.sort.newest": "Mới nhất",
    "sp.sort.price_asc": "Giá thấp → cao",
    "sp.sort.price_desc": "Giá cao → thấp",
    "sp.price": "Khoảng giá",
    "sp.condition": "Tình trạng",
    "sp.brand": "Hãng sản xuất",
    "sp.location": "Khu vực",
    "sp.apply": "Áp dụng",
    "sp.clear": "Xoá lọc",
    "sp.promoted": "Tin tài trợ",

    // product page
    "pp.specs": "Thông số kỹ thuật",
    "pp.desc": "Mô tả chi tiết",
    "pp.posted": "Đăng ngày",
    "pp.views": "lượt xem",
    "pp.safety": "Mẹo an toàn: Kiểm tra kỹ thiết bị và gặp trực tiếp trước khi thanh toán.",
    "pp.report": "Báo cáo tin này",
    "pp.similar": "Tin tương tự",

    // post listing
    "post.title": "Đăng tin bán thiết bị",
    "post.sub": "Điền thông tin bên dưới. Trường có dấu * là bắt buộc.",
    "post.photos": "Hình ảnh",
    "post.photos.hint": "Tải lên tối đa 10 ảnh. Ảnh đầu tiên là ảnh bìa.",
    "post.basic": "Thông tin cơ bản",
    "post.name": "Tên thiết bị",
    "post.category": "Danh mục",
    "post.condition": "Tình trạng",
    "post.price": "Giá bán (VNĐ)",
    "post.desc": "Mô tả",
    "post.specs": "Thông số kỹ thuật",
    "post.contact": "Thông tin liên hệ",
    "post.submit": "Đăng tin",
    "post.draft": "Lưu nháp",
    "post.boostq": "Muốn tin lên đầu? Chọn gói quảng cáo sau khi đăng.",

    // dashboard
    "dash.title": "Quản lý tin đăng",
    "dash.new": "Đăng tin mới",
    "dash.active": "Đang hiển thị",
    "dash.pending": "Chờ duyệt",
    "dash.views": "Lượt xem (30 ngày)",
    "dash.leads": "Liên hệ nhận được",
    "dash.mylistings": "Tin của tôi",
    "dash.boost": "Đẩy tin",
    "dash.edit": "Sửa",
    "dash.status": "Trạng thái",

    // admin
    "adm.title": "Bảng quản trị",
    "adm.queue": "Hàng chờ duyệt",
    "adm.reports": "Tin bị báo cáo",
    "adm.users": "Người dùng",
    "adm.approve": "Duyệt",
    "adm.reject": "Từ chối",
    "adm.delete": "Xoá",
    "adm.view": "Xem",
    "adm.pending_total": "Tin chờ duyệt",
    "adm.reported_total": "Tin bị báo cáo",
    "adm.live_total": "Tin đang hiển thị",
    "adm.users_total": "Người dùng",

    // auth
    "auth.login": "Đăng nhập",
    "auth.register": "Đăng ký",
    "auth.email": "Email",
    "auth.password": "Mật khẩu",
    "auth.remember": "Ghi nhớ đăng nhập",
    "auth.forgot": "Quên mật khẩu?",
    "auth.google": "Tiếp tục với Google",
    "auth.or": "hoặc",
    "auth.noaccount": "Chưa có tài khoản?",
    "auth.needaccount": "Bạn cần tài khoản để đăng tin bán thiết bị."
  },
  en: {
    "top.support": "Support: 1900 6060",
    "top.help": "Help",
    "nav.buy": "Buy & Sell",
    "nav.categories": "Categories",
    "nav.dealers": "Dealers",
    "nav.pricing": "Advertising",
    "nav.login": "Log in",
    "nav.post": "Post listing",
    "nav.dashboard": "My listings",
    "search.what": "What machine are you looking for?",
    "search.what.label": "Keyword",
    "search.cat.label": "Category",
    "search.loc.label": "Location",
    "search.cat.all": "All categories",
    "search.loc.all": "Nationwide",
    "search.btn": "Search",
    "search.popular": "Popular:",

    "hero.eyebrow": "Vietnam's #1 construction machinery marketplace",
    "hero.title": "Buy & sell construction machinery & equipment",
    "hero.sub": "Tens of thousands of listings from trusted suppliers. Find the right equipment and contact the seller directly — no middleman.",
    "hero.stat1": "Live listings",
    "hero.stat2": "Suppliers",
    "hero.stat3": "Deals / month",

    "cats.title": "Equipment categories",
    "cats.sub": "Pick the type of machine you need",
    "feat.title": "Featured listings",
    "feat.sub": "Promoted, priority placement",
    "feat.viewall": "View all",
    "recent.title": "Recently posted",
    "trust.title": "Why choose us",
    "trust.1.t": "Verified sellers",
    "trust.1.d": "Suppliers verify their business license before listing.",
    "trust.2.t": "Contact directly",
    "trust.2.d": "Call or message the seller directly, with no broker fees.",
    "trust.3.t": "Moderated listings",
    "trust.3.d": "Our admin team reviews and removes violating listings daily.",
    "seller.eyebrow": "For sellers",
    "seller.title": "List your equipment and reach thousands of buyers",
    "seller.sub": "Create a listing free in 2 minutes. Push it to the top of search with an advertising package.",
    "seller.cta": "Post a free listing",
    "seller.cta2": "See ad packages",

    "lbl.boosted": "Promoted",
    "lbl.verified": "Verified",
    "lbl.new": "New",
    "lbl.used": "Used",
    "lbl.contact": "Contact seller",
    "lbl.call": "Call now",
    "lbl.message": "Message",
    "lbl.negotiable": "Negotiable",

    "foot.about": "Vietnam's leading marketplace for buying, selling and renting construction machinery and equipment.",
    "foot.explore": "Explore",
    "foot.forsellers": "For sellers",
    "foot.support": "Support",
    "foot.rights": "© 2026 MayXayDung. All rights reserved.",

    "sp.results": "results for",
    "sp.filters": "Filters",
    "sp.sort": "Sort",
    "sp.sort.relevant": "Most relevant",
    "sp.sort.newest": "Newest",
    "sp.sort.price_asc": "Price low → high",
    "sp.sort.price_desc": "Price high → low",
    "sp.price": "Price range",
    "sp.condition": "Condition",
    "sp.brand": "Brand",
    "sp.location": "Location",
    "sp.apply": "Apply",
    "sp.clear": "Clear filters",
    "sp.promoted": "Sponsored",

    "pp.specs": "Specifications",
    "pp.desc": "Description",
    "pp.posted": "Posted",
    "pp.views": "views",
    "pp.safety": "Safety tip: Always inspect the equipment and meet in person before paying.",
    "pp.report": "Report this listing",
    "pp.similar": "Similar listings",

    "post.title": "Post an equipment listing",
    "post.sub": "Fill in the details below. Fields marked * are required.",
    "post.photos": "Photos",
    "post.photos.hint": "Upload up to 10 photos. The first is the cover image.",
    "post.basic": "Basic information",
    "post.name": "Equipment name",
    "post.category": "Category",
    "post.condition": "Condition",
    "post.price": "Price (VND)",
    "post.desc": "Description",
    "post.specs": "Specifications",
    "post.contact": "Contact information",
    "post.submit": "Publish listing",
    "post.draft": "Save draft",
    "post.boostq": "Want the top spot? Choose an ad package after posting.",

    "dash.title": "Manage listings",
    "dash.new": "New listing",
    "dash.active": "Active",
    "dash.pending": "Pending review",
    "dash.views": "Views (30 days)",
    "dash.leads": "Leads received",
    "dash.mylistings": "My listings",
    "dash.boost": "Boost",
    "dash.edit": "Edit",
    "dash.status": "Status",

    "adm.title": "Admin panel",
    "adm.queue": "Moderation queue",
    "adm.reports": "Reported listings",
    "adm.users": "Users",
    "adm.approve": "Approve",
    "adm.reject": "Reject",
    "adm.delete": "Delete",
    "adm.view": "View",
    "adm.pending_total": "Pending review",
    "adm.reported_total": "Reported",
    "adm.live_total": "Live listings",
    "adm.users_total": "Users",

    "auth.login": "Log in",
    "auth.register": "Sign up",
    "auth.email": "Email",
    "auth.password": "Password",
    "auth.remember": "Remember me",
    "auth.forgot": "Forgot password?",
    "auth.google": "Continue with Google",
    "auth.or": "or",
    "auth.noaccount": "Don't have an account?",
    "auth.needaccount": "You need an account to post equipment for sale."
  }
};

function applyLang(lang) {
  const dict = I18N[lang] || I18N.vi;
  document.documentElement.lang = lang;
  document.querySelectorAll('[data-i18n]').forEach(el => {
    const k = el.getAttribute('data-i18n');
    if (dict[k] != null) el.textContent = dict[k];
  });
  document.querySelectorAll('[data-i18n-ph]').forEach(el => {
    const k = el.getAttribute('data-i18n-ph');
    if (dict[k] != null) el.setAttribute('placeholder', dict[k]);
  });
  document.querySelectorAll('.lang-toggle button').forEach(b => {
    b.classList.toggle('active', b.dataset.lang === lang);
  });
  try { localStorage.setItem('mxd-lang', lang); } catch (e) {}
}

function initLang() {
  let lang = 'vi';
  try { lang = localStorage.getItem('mxd-lang') || 'vi'; } catch (e) {}
  applyLang(lang);
  document.querySelectorAll('.lang-toggle button').forEach(b => {
    b.addEventListener('click', () => applyLang(b.dataset.lang));
  });
}

/* Build a slide-in mobile drawer on any page that has a .site-header.
   Keeps mobile navigation consistent without editing every HTML file. */
function buildMobileNav() {
  const header = document.querySelector('.site-header');
  if (!header) return; // admin uses its own sidebar layout

  // Only browsing pages hide primary navigation on mobile and need the drawer.
  // Focused post-login pages (post-listing, dashboard) already expose their
  // actions directly in the header, so we skip the burger there.
  const needsDrawer = document.getElementById('burger') || header.querySelector('.nav-link, .searchbar');
  if (!needsDrawer) return;

  const links = [
    { href: 'search.html',       i18n: 'nav.buy',        icon: 'tag',        text: 'Mua bán' },
    { href: 'search.html',       i18n: 'nav.categories', icon: 'layout-grid',text: 'Danh mục' },
    { href: '#',                 i18n: 'nav.dealers',    icon: 'store',      text: 'Nhà cung cấp' },
    { href: '#',                 i18n: 'nav.pricing',    icon: 'zap',        text: 'Gói quảng cáo' },
    { href: 'dashboard.html',    i18n: 'nav.dashboard',  icon: 'layout-dashboard', text: 'Quản lý tin' },
    { href: 'login.html',        i18n: 'nav.login',      icon: 'log-in',     text: 'Đăng nhập' }
  ];

  const drawer = document.createElement('div');
  drawer.className = 'mobile-drawer';
  drawer.innerHTML =
    '<div class="scrim" data-close></div>' +
    '<nav class="panel" aria-label="Mobile menu">' +
      '<div class="drawer-head">' +
        '<span class="brand-mark" style="font-size:1.1rem"><span class="brand-logo" style="width:32px;height:32px"><i data-lucide="hard-hat" class="w-4 h-4"></i></span>MayXayDung</span>' +
        '<button data-close aria-label="Close menu" style="border:none;background:transparent;cursor:pointer;color:var(--steel-500)"><i data-lucide="x" class="w-6 h-6"></i></button>' +
      '</div>' +
      links.map(l => '<a href="' + l.href + '"><i data-lucide="' + l.icon + '" class="w-5 h-5"></i><span data-i18n="' + l.i18n + '">' + l.text + '</span></a>').join('') +
      '<div class="lang-toggle mt-4" style="align-self:flex-start"><button data-lang="vi">VI</button><button data-lang="en">EN</button></div>' +
    '</nav>';
  document.body.appendChild(drawer);

  // ensure a burger button exists in the header's right-side cluster
  let burger = document.getElementById('burger');
  if (!burger) {
    // use the header's MAIN nav row (last direct-child container), not the hidden top-bar
    const rows = header.querySelectorAll(':scope > .container-wide, :scope > .container');
    const mainRow = rows[rows.length - 1] || header;
    const cluster = mainRow.querySelector(':scope > div:last-child') || mainRow;
    burger = document.createElement('button');
    burger.id = 'burger';
    burger.className = 'mobile-nav btn btn-ghost';
    burger.setAttribute('aria-label', 'Menu');
    burger.innerHTML = '<i data-lucide="menu" class="w-5 h-5"></i>';
    if (cluster) cluster.appendChild(burger);
  }

  const open = () => { drawer.classList.add('open'); document.body.style.overflow = 'hidden'; };
  const close = () => { drawer.classList.remove('open'); document.body.style.overflow = ''; };
  burger.addEventListener('click', open);
  drawer.querySelectorAll('[data-close]').forEach(el => el.addEventListener('click', close));
  document.addEventListener('keydown', e => { if (e.key === 'Escape') close(); }); // a11y escape route

  if (window.lucide) lucide.createIcons();
}

document.addEventListener('DOMContentLoaded', () => {
  buildMobileNav();
  initLang(); // wires ALL .lang-toggle buttons, including the drawer's

  // favourite toggle
  document.addEventListener('click', e => {
    const fav = e.target.closest('.corner-fav');
    if (fav) { fav.classList.toggle('is-fav'); fav.style.color = fav.classList.contains('is-fav') ? 'var(--accent)' : ''; }
  });
});
