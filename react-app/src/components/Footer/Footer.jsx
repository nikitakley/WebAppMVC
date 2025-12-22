import './Footer.css'

export default function Footer() {
  return (
    <footer className='footer'>
      <p>© {new Date().getFullYear()} FoodDel. Все права защищены.</p>
    </footer>
  );
}