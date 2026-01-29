import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <footer class="footer">
      <div class="footer-container">
        <!-- Brand -->
        <div class="footer-brand">
          <h3 class="brand-name">Explore with Soul</h3>
          <p class="brand-tagline">Discover the world's best tours and experiences</p>
        </div>

        <!-- Footer Links -->
        <div class="footer-links">
          <!-- Support -->
          <div class="footer-column">
            <h4>Support</h4>
            <ul>
              <li><a href="#">Help Center</a></li>
              <li><a href="#">FAQ</a></li>
              <li><a href="#">Contact Us</a></li>
            </ul>
          </div>

          <!-- Social -->
          <div class="footer-column">
            <h4>Social</h4>
            <ul>
              <li><a href="#">Instagram</a></li>
              <li><a href="#">Facebook</a></li>
              <li><a href="#">Twitter</a></li>
            </ul>
          </div>

          <!-- Company -->
          <div class="footer-column">
            <h4>Company</h4>
            <ul>
              <li><a href="#">About Us</a></li>
              <li><a href="#">Careers</a></li>
              <li><a href="#">Blog</a></li>
            </ul>
          </div>

          <!-- Legal -->
          <div class="footer-column">
            <h4>Legal</h4>
            <ul>
              <li><a href="#">Terms of Service</a></li>
              <li><a href="#">Privacy Policy</a></li>
              <li><a href="#">Cookie Policy</a></li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Bottom Bar -->
      <div class="footer-bottom">
        <p>&copy; {{ currentYear }} Explore with Soul. All rights reserved.</p>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: var(--color-primary-dark);
      color: var(--color-text-light);
      margin-top: auto;
    }

    .footer-container {
      max-width: 1280px;
      margin: 0 auto;
      padding: var(--spacing-xxl) var(--spacing-lg);
      display: grid;
      grid-template-columns: 1fr 2fr;
      gap: var(--spacing-xxl);
    }

    @media (max-width: 768px) {
      .footer-container {
        grid-template-columns: 1fr;
        text-align: center;
      }
    }

    .footer-brand {
      padding-right: var(--spacing-xl);
    }

    .brand-name {
      font-size: var(--font-size-xl);
      margin: 0 0 var(--spacing-sm) 0;
      color: #ffffff;
    }

    .brand-tagline {
      font-size: var(--font-size-sm);
      color: rgba(255, 255, 255, 0.7);
      margin: 0;
      line-height: 1.6;
    }

    .footer-links {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--spacing-lg);
    }

    @media (max-width: 768px) {
      .footer-links {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    @media (max-width: 480px) {
      .footer-links {
        grid-template-columns: 1fr;
      }
    }

    .footer-column h4 {
      font-size: var(--font-size-sm);
      text-transform: uppercase;
      letter-spacing: 1px;
      margin: 0 0 var(--spacing-md) 0;
      color: rgba(255, 255, 255, 0.9);
    }

    .footer-column ul {
      list-style: none;
      padding: 0;
      margin: 0;
    }

    .footer-column li {
      margin-bottom: var(--spacing-sm);
    }

    .footer-column a {
      color: rgba(255, 255, 255, 0.7);
      text-decoration: none;
      font-size: var(--font-size-sm);
      transition: color var(--transition-fast);
    }

    .footer-column a:hover {
      color: var(--color-accent);
    }

    .footer-bottom {
      border-top: 1px solid rgba(255, 255, 255, 0.1);
      padding: var(--spacing-lg);
      text-align: center;
    }

    .footer-bottom p {
      margin: 0;
      font-size: var(--font-size-sm);
      color: rgba(255, 255, 255, 0.6);
    }
  `]
})
export class FooterComponent {
  get currentYear(): number {
    return new Date().getFullYear();
  }
}
