import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-manage',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './manage.html',
  styleUrl: './manage.css'
})
export class Manage {}
