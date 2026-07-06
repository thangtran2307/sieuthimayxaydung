/* Mock listing data shared across prototype pages */
const LISTINGS = [
  { title:"Máy đào Komatsu PC200-8 đời 2019", price:"1.250.000.000 ₫", cond:"used", verified:true, boost:true,  loc:"TP.HCM",  time:"2 giờ trước", img:"https://placehold.co/600x450/1e293b/ffffff?text=Komatsu+PC200" },
  { title:"Xe nâng dầu Toyota 3 tấn 8FD30", price:"420.000.000 ₫",  cond:"used", verified:true, boost:true,  loc:"Hà Nội",   time:"5 giờ trước", img:"https://placehold.co/600x450/334155/ffffff?text=Toyota+8FD30" },
  { title:"Cần cẩu tháp Zoomlion TC6013 mới 100%", price:"Liên hệ",   cond:"new",  verified:true, boost:true,  loc:"Bình Dương",time:"1 ngày trước",img:"https://placehold.co/600x450/1e3a5f/ffffff?text=Tower+Crane" },
  { title:"Máy trộn bê tông tự hành 4 khối", price:"680.000.000 ₫",  cond:"new",  verified:false,boost:true,  loc:"Đà Nẵng",  time:"1 ngày trước",img:"https://placehold.co/600x450/475569/ffffff?text=Concrete+Mixer" },
  { title:"Xe lu rung Sakai SV520 12 tấn",   price:"890.000.000 ₫",  cond:"used", verified:true, boost:true,  loc:"Đồng Nai",  time:"2 ngày trước",img:"https://placehold.co/600x450/0f172a/ffffff?text=Sakai+Roller" },
  { title:"Máy xúc lật Liugong 856H",        price:"720.000.000 ₫",  cond:"used", verified:true, boost:false, loc:"Hải Phòng", time:"3 giờ trước", img:"https://placehold.co/600x450/1e293b/ffffff?text=Wheel+Loader" },
  { title:"Xe nâng điện Hangcha 2 tấn",      price:"210.000.000 ₫",  cond:"new",  verified:false,boost:false, loc:"TP.HCM",   time:"6 giờ trước", img:"https://placehold.co/600x450/334155/ffffff?text=Electric+Forklift" },
  { title:"Máy đào mini Kubota U-30",        price:"absurd", cond:"used", verified:true, boost:false, loc:"Long An",   time:"8 giờ trước", img:"https://placehold.co/600x450/1e3a5f/ffffff?text=Kubota+U30" },
  { title:"Cẩu bánh xích Kobelco 55 tấn",    price:"3.400.000.000 ₫",cond:"used", verified:true, boost:false, loc:"Bà Rịa",    time:"12 giờ trước",img:"https://placehold.co/600x450/475569/ffffff?text=Crawler+Crane" },
  { title:"Máy phát điện Cummins 250kVA",    price:"340.000.000 ₫",  cond:"used", verified:false,boost:false, loc:"Hà Nội",    time:"1 ngày trước",img:"https://placehold.co/600x450/0f172a/ffffff?text=Generator" },
  { title:"Xe ben Howo 3 chân 15 khối",      price:"1.050.000.000 ₫",cond:"used", verified:true, boost:false, loc:"TP.HCM",    time:"1 ngày trước",img:"https://placehold.co/600x450/1e293b/ffffff?text=Dump+Truck" },
  { title:"Máy khoan cọc nhồi Sany SR155",   price:"Liên hệ",        cond:"used", verified:true, boost:false, loc:"Bình Dương",time:"2 ngày trước",img:"https://placehold.co/600x450/334155/ffffff?text=Piling+Rig" },
];
// fix stray value
LISTINGS[7].price = "185.000.000 ₫";
