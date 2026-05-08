import React from 'react';
import './Footer.css';

function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="App-footer">
      <div className="footer-content">
        <p>&copy; {currentYear} Employee Management System. All rights reserved.</p>
        <div className="footer-links">
          <a href="#privacy">Privacy Policy</a>
          <span className="separator">|</span>
          <a href="#terms">Terms of Service</a>
          <span className="separator">|</span>
          <a href="#support">Support</a>
        </div>
      </div>
    </footer>
  );
}

export default Footer;
