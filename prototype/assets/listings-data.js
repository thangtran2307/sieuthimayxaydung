/* Mock data shared across prototype pages */

const CATEGORIES = [
  { id:'excavator', label:'Máy đào', labelEn:'Excavators', icon:'truck', count:2410,
    subs:[
      {id:'crawler',    label:'Máy đào bánh xích', labelEn:'Crawler Excavator',  count:1240},
      {id:'wheeled',    label:'Máy đào bánh lốp',  labelEn:'Wheeled Excavator',  count:810},
      {id:'mini',       label:'Máy đào mini',       labelEn:'Mini Excavator',     count:360},
    ]},
  { id:'forklift', label:'Xe nâng', labelEn:'Forklifts', icon:'forklift', count:1880,
    subs:[
      {id:'diesel_fl',  label:'Xe nâng dầu',        labelEn:'Diesel Forklift',    count:920},
      {id:'electric_fl',label:'Xe nâng điện',        labelEn:'Electric Forklift',  count:640},
      {id:'reach_fl',   label:'Xe nâng cao',         labelEn:'Reach Truck',        count:320},
    ]},
  { id:'crane', label:'Cần cẩu', labelEn:'Cranes', icon:'construction', count:960,
    subs:[
      {id:'tower_cr',   label:'Cần cẩu tháp',        labelEn:'Tower Crane',        count:380},
      {id:'mobile_cr',  label:'Cần cẩu di động',      labelEn:'Mobile Crane',       count:420},
      {id:'crawler_cr', label:'Cần cẩu bánh xích',    labelEn:'Crawler Crane',      count:160},
    ]},
  { id:'concrete', label:'Máy trộn & bơm', labelEn:'Concrete Equipment', icon:'cog', count:1120,
    subs:[
      {id:'mixer',      label:'Máy trộn bê tông',    labelEn:'Concrete Mixer',     count:540},
      {id:'truck_mix',  label:'Xe bồn bê tông',       labelEn:'Transit Mixer',      count:380},
      {id:'pump',       label:'Máy bơm bê tông',      labelEn:'Concrete Pump',      count:200},
    ]},
  { id:'road', label:'Máy lu & làm đường', labelEn:'Road Equipment', icon:'mountain', count:740,
    subs:[
      {id:'roller',     label:'Xe lu',                labelEn:'Road Roller',        count:340},
      {id:'paver',      label:'Máy rải nhựa',         labelEn:'Asphalt Paver',      count:260},
      {id:'cutter',     label:'Máy cắt đường',        labelEn:'Road Cutter',        count:140},
    ]},
  { id:'parts', label:'Phụ tùng & linh kiện', labelEn:'Parts & Components', icon:'wrench', count:5300,
    subs:[
      {id:'exc_parts',  label:'Phụ tùng máy đào',    labelEn:'Excavator Parts',    count:2100},
      {id:'flft_parts', label:'Phụ tùng xe nâng',     labelEn:'Forklift Parts',     count:1800},
      {id:'other_parts',label:'Phụ tùng khác',        labelEn:'Other Parts',        count:1400},
    ]},
];

const SELLERS = [
  { id:'tan-minh', name:'Tân Minh Machinery', initials:'TM', verified:true,
    loc:'Quận 9, TP.HCM', since:'2019', listings:128, views:'240.000', response:'1 giờ',
    cover:'https://placehold.co/1200x320/1e3a5f/ffffff?text=Tân+Minh+Machinery',
    desc:'Chuyên cung cấp máy đào, xe nâng và thiết bị nâng hạ nhập khẩu trực tiếp từ Nhật Bản và Hàn Quốc. Hơn 5 năm kinh nghiệm, bảo hành 12 tháng toàn bộ sản phẩm. Có xưởng dịch vụ và kho phụ tùng tại TP.HCM.',
    phone:'0909 •••• 88', cats:['Máy đào','Xe nâng','Cần cẩu'] },
  { id:'hp-equipment', name:'HP Construction Equipment', initials:'HP', verified:true,
    loc:'Hải An, Hải Phòng', since:'2017', listings:84, views:'180.000', response:'2 giờ',
    cover:'https://placehold.co/1200x320/334155/ffffff?text=HP+Construction+Equipment',
    desc:'Nhà phân phối chính thức Komatsu và Hitachi tại miền Bắc. Xưởng bảo dưỡng 2.000m² với kỹ thuật viên chuyên nghiệp. Cung cấp phụ tùng chính hãng toàn quốc.',
    phone:'0912 •••• 89', cats:['Máy đào','Phụ tùng'] },
];

const LISTINGS = [
  { title:"Máy đào Komatsu PC200-8 đời 2019",  price:"1.250.000.000 ₫", cond:"used", verified:true,  boost:true,  loc:"TP.HCM",    time:"2 giờ trước",   cat:"excavator", subcat:"crawler",     seller:"tan-minh",   img:"https://placehold.co/600x450/1e293b/ffffff?text=Komatsu+PC200" },
  { title:"Xe nâng dầu Toyota 3 tấn 8FD30",    price:"420.000.000 ₫",   cond:"used", verified:true,  boost:true,  loc:"Hà Nội",    time:"5 giờ trước",   cat:"forklift",  subcat:"diesel_fl",   seller:"hp-equipment",img:"https://placehold.co/600x450/334155/ffffff?text=Toyota+8FD30" },
  { title:"Cần cẩu tháp Zoomlion TC6013 mới",   price:"Liên hệ",         cond:"new",  verified:true,  boost:true,  loc:"Bình Dương", time:"1 ngày trước",  cat:"crane",     subcat:"tower_cr",    seller:"tan-minh",   img:"https://placehold.co/600x450/1e3a5f/ffffff?text=Tower+Crane" },
  { title:"Máy trộn bê tông tự hành 4 khối",    price:"680.000.000 ₫",   cond:"new",  verified:false, boost:true,  loc:"Đà Nẵng",   time:"1 ngày trước",  cat:"concrete",  subcat:"mixer",       seller:"tan-minh",   img:"https://placehold.co/600x450/475569/ffffff?text=Concrete+Mixer" },
  { title:"Xe lu rung Sakai SV520 12 tấn",      price:"890.000.000 ₫",   cond:"used", verified:true,  boost:true,  loc:"Đồng Nai",  time:"2 ngày trước",  cat:"road",      subcat:"roller",      seller:"hp-equipment",img:"https://placehold.co/600x450/0f172a/ffffff?text=Sakai+Roller" },
  { title:"Máy xúc lật Liugong 856H",           price:"720.000.000 ₫",   cond:"used", verified:true,  boost:false, loc:"Hải Phòng", time:"3 giờ trước",   cat:"excavator", subcat:"wheeled",     seller:"hp-equipment",img:"https://placehold.co/600x450/1e293b/ffffff?text=Wheel+Loader" },
  { title:"Xe nâng điện Hangcha 2 tấn mới",     price:"210.000.000 ₫",   cond:"new",  verified:false, boost:false, loc:"TP.HCM",    time:"6 giờ trước",   cat:"forklift",  subcat:"electric_fl", seller:"tan-minh",   img:"https://placehold.co/600x450/334155/ffffff?text=Electric+Forklift" },
  { title:"Máy đào mini Kubota U-30",           price:"185.000.000 ₫",   cond:"used", verified:true,  boost:false, loc:"Long An",   time:"8 giờ trước",   cat:"excavator", subcat:"mini",        seller:"tan-minh",   img:"https://placehold.co/600x450/1e3a5f/ffffff?text=Kubota+U30" },
  { title:"Cẩu bánh xích Kobelco 55 tấn",       price:"3.400.000.000 ₫", cond:"used", verified:true,  boost:false, loc:"Bà Rịa",    time:"12 giờ trước",  cat:"crane",     subcat:"crawler_cr",  seller:"hp-equipment",img:"https://placehold.co/600x450/475569/ffffff?text=Crawler+Crane" },
  { title:"Máy phát điện Cummins 250kVA",       price:"340.000.000 ₫",   cond:"used", verified:false, boost:false, loc:"Hà Nội",    time:"1 ngày trước",  cat:"parts",     subcat:"other_parts", seller:"hp-equipment",img:"https://placehold.co/600x450/0f172a/ffffff?text=Generator" },
  { title:"Xe ben Howo 3 chân 15 khối",         price:"1.050.000.000 ₫", cond:"used", verified:true,  boost:false, loc:"TP.HCM",    time:"1 ngày trước",  cat:"road",      subcat:"paver",       seller:"tan-minh",   img:"https://placehold.co/600x450/1e293b/ffffff?text=Dump+Truck" },
  { title:"Máy khoan cọc nhồi Sany SR155",      price:"Liên hệ",         cond:"used", verified:true,  boost:false, loc:"Bình Dương", time:"2 ngày trước",  cat:"excavator", subcat:"crawler",     seller:"hp-equipment",img:"https://placehold.co/600x450/334155/ffffff?text=Piling+Rig" },
];
