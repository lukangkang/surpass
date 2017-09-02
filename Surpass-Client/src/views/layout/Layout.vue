<template>
	<div class="app-wrapper" :class="{hideSidebar:!sidebar.opened}">
		<sidebar class="sidebar-container"></sidebar>
		<div class="main-container">
			<navbar></navbar>
			<app-main></app-main>
		</div>
	</div>
</template>

<script>
import { Navbar, Sidebar, AppMain } from 'views/layout'