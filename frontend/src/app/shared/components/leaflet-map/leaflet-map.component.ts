import { 
  Component, 
  Input, 
  Output, 
  EventEmitter, 
  AfterViewInit, 
  OnDestroy, 
  OnChanges, 
  SimpleChanges,
  ElementRef,
  ViewChild,
  PLATFORM_ID,
  inject
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { KeyPointDto } from '../../models';

export interface MapCoordinates {
  latitude: number;
  longitude: number;
}

@Component({
  selector: 'app-leaflet-map',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div #mapContainer class="map-container" [style.height]="height"></div>
  `,
  styles: [`
    .map-container {
      width: 100%;
      min-height: 300px;
      border-radius: 8px;
      border: 1px solid #ddd;
    }
  `]
})
export class LeafletMapComponent implements AfterViewInit, OnDestroy, OnChanges {
  @ViewChild('mapContainer') mapContainer!: ElementRef;
  
  @Input() height = '400px';
  @Input() center: MapCoordinates = { latitude: 44.8176, longitude: 20.4633 }; // Belgrade default
  @Input() zoom = 13;
  @Input() keyPoints: KeyPointDto[] = [];
  @Input() clickable = false;
  @Input() showPolyline = false;
  
  @Output() mapClick = new EventEmitter<MapCoordinates>();
  
  private platformId = inject(PLATFORM_ID);
  private map: any;
  private L: any;
  private markers: any[] = [];
  private polyline: any;
  private clickMarker: any;

  async ngAfterViewInit(): Promise<void> {
    if (isPlatformBrowser(this.platformId)) {
      await this.initMap();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.map && this.L) {
      if (changes['keyPoints']) {
        this.updateMarkers();
      }
      if (changes['center'] && !changes['center'].firstChange) {
        this.map.setView([this.center.latitude, this.center.longitude], this.zoom);
      }
    }
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  private async initMap(): Promise<void> {
    const leaflet = await import('leaflet');
    this.L = leaflet.default || leaflet;

    this.map = this.L.map(this.mapContainer.nativeElement).setView(
      [this.center.latitude, this.center.longitude],
      this.zoom
    );

    this.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);

    if (this.clickable) {
      this.map.on('click', (e: any) => {
        const coords: MapCoordinates = {
          latitude: e.latlng.lat,
          longitude: e.latlng.lng
        };
        
        if (this.clickMarker) {
          this.clickMarker.setLatLng([coords.latitude, coords.longitude]);
        } else {
          this.clickMarker = this.L.marker([coords.latitude, coords.longitude], {
            icon: this.createClickIcon()
          }).addTo(this.map);
        }
        
        this.mapClick.emit(coords);
      });
    }

    this.updateMarkers();
  }

  private updateMarkers(): void {
    if (!this.map || !this.L) return;

    // Clear existing markers
    this.markers.forEach(marker => this.map.removeLayer(marker));
    this.markers = [];

    if (this.polyline) {
      this.map.removeLayer(this.polyline);
      this.polyline = null;
    }

    // Add new markers
    const sortedPoints = [...this.keyPoints].sort((a, b) => a.order - b.order);
    const latLngs: [number, number][] = [];

    sortedPoints.forEach((kp, index) => {
      const marker = this.L.marker([kp.latitude, kp.longitude], {
        icon: this.createNumberedIcon(index + 1)
      })
        .addTo(this.map)
        .bindPopup(`<strong>${kp.name}</strong><br>${kp.description}`);
      
      this.markers.push(marker);
      latLngs.push([kp.latitude, kp.longitude]);
    });

    // Add polyline if enabled and we have at least 2 points
    if (this.showPolyline && latLngs.length >= 2) {
      this.polyline = this.L.polyline(latLngs, {
        color: '#3388ff',
        weight: 3,
        opacity: 0.7
      }).addTo(this.map);
    }

    // Fit bounds to show all markers
    if (latLngs.length > 0) {
      const bounds = this.L.latLngBounds(latLngs);
      this.map.fitBounds(bounds, { padding: [50, 50] });
    }
  }

  private createNumberedIcon(number: number): any {
    return this.L.divIcon({
      className: 'custom-marker',
      html: `<div style="
        background-color: #3388ff;
        color: white;
        width: 28px;
        height: 28px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: bold;
        font-size: 14px;
        border: 2px solid white;
        box-shadow: 0 2px 4px rgba(0,0,0,0.3);
      ">${number}</div>`,
      iconSize: [28, 28],
      iconAnchor: [14, 14]
    });
  }

  private createClickIcon(): any {
    return this.L.divIcon({
      className: 'click-marker',
      html: `<div style="
        background-color: #e74c3c;
        color: white;
        width: 32px;
        height: 32px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 18px;
        border: 3px solid white;
        box-shadow: 0 2px 6px rgba(0,0,0,0.4);
      ">+</div>`,
      iconSize: [32, 32],
      iconAnchor: [16, 16]
    });
  }

  clearClickMarker(): void {
    if (this.clickMarker && this.map) {
      this.map.removeLayer(this.clickMarker);
      this.clickMarker = null;
    }
  }
}
