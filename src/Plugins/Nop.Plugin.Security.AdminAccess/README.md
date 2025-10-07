# Admin Access Control Plugin

## Overview
This plugin provides enhanced security for the NopCommerce admin area by restricting access based on IP whitelist and/or a secret access key.

## Features

### 1. IP Whitelist
- Restrict admin access to specific IP addresses
- Supports multiple IPs (comma-separated)
- Automatically detects client IP (even behind proxies)
- Use `*` to allow all IPs (not recommended for production)

### 2. Secret Key Access
- Generate a secure random access key
- Access admin via: `/admin?key=YOUR_SECRET_KEY`
- One-time authentication sets a secure cookie
- Regenerate key anytime from the plugin configuration

### 3. Persistent Authentication
- After successful validation (IP or key), a secure HTTP-only cookie is set
- Cookie expiration is configurable (default: 24 hours)
- Secure, SameSite=Strict cookie prevents CSRF attacks

### 4. Custom Access Denied Message
- Customize the message shown to unauthorized users
- Beautiful access denied page with professional styling

## Installation

1. The plugin is located in: `src/Presentation/Nop.Web/Themes/Maiba/Plugins/Nop.Plugin.Security.AdminAccess/`
2. Build the solution
3. Navigate to **Admin > Configuration > Local Plugins**
4. Find "Admin Access Control" and click **Install**
5. After installation, click **Configure**

## Configuration

### Settings:

- **Enable IP Whitelist**: Turn IP restriction on/off
- **Allowed IP Addresses**: Comma-separated list of IPs (e.g., `192.168.1.1,10.0.0.5`)
- **Enable Secret Key**: Turn secret key access on/off
- **Secret Access Key**: Auto-generated 32-character key (can be regenerated)
- **Cookie Expiration**: How long the auth cookie remains valid (hours)
- **Access Denied Message**: Custom message for unauthorized users

### How to Access Admin:

#### Method 1: IP Whitelist
1. Add your IP address to the "Allowed IP Addresses" field
2. Access `/admin` normally

#### Method 2: Secret Key
1. Copy the access URL from the configuration page
2. Share it securely with authorized admins
3. Example: `https://yoursite.com/admin?key=abc123xyz`
4. After first access, the cookie is set for configured duration

### Security Best Practices:

1. **Use both IP and Key**: Enable both methods for maximum security
2. **Regenerate keys regularly**: Click "Regenerate" to create new access keys
3. **Monitor access**: Check server logs for unauthorized attempts
4. **Use HTTPS**: Always use SSL/TLS in production
5. **Limit cookie expiration**: Use shorter cookie expiration for sensitive environments

## Troubleshooting

### Can't access admin after installation:
1. The plugin may have activated with default settings
2. Check your current IP address: https://whatismyipaddress.com/
3. Add your IP to the whitelist via database or disable the plugin temporarily

### IP not matching:
- If behind a proxy/load balancer, the plugin checks `X-Forwarded-For` and `X-Real-IP` headers
- Contact your hosting provider if IP detection isn't working

### Lost secret key:
- Access the plugin configuration via database directly
- Or temporarily disable the plugin to regain access

## Development

The plugin follows NopCommerce plugin architecture:
- **Controllers**: Handle configuration UI
- **Infrastructure**: Filter that intercepts admin requests
- **Models**: Configuration view models
- **Views**: Razor configuration pages

## Version Control

This plugin is stored in the theme's Plugins folder for easier version control with the Maiba theme.

## License

Copyright © Maiba Jewelry - All Rights Reserved

